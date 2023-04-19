// Copyright 2022-2023 Deveel
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Webhooks {
    class WebhookDelegatedReceiverMiddleware<TWebhook> where TWebhook : class {
		private readonly RequestDelegate next;
		private readonly Func<HttpContext, TWebhook, CancellationToken, Task>? asyncHandler;
		private readonly Action<HttpContext, TWebhook>? syncHandler;

		public WebhookDelegatedReceiverMiddleware(RequestDelegate next,
			Func<HttpContext, TWebhook, CancellationToken, Task> handler) {
			this.next = next;
			asyncHandler = handler;
		}

		public WebhookDelegatedReceiverMiddleware(RequestDelegate next,
			Action<HttpContext, TWebhook> handler) {
			this.next = next;
			syncHandler = handler;
		}


		public async Task InvokeAsync(HttpContext context) {
			try {
				var receiver = context.RequestServices.GetService<IWebhookReceiver<TWebhook>>();
				if (receiver == null) {
					await next(context);
				} else {
					var result = await receiver.ReceiveAsync(context.Request, context.RequestAborted);

					if (result.SignatureValid != null && !result.SignatureValid.Value) {
						// TODO: get this from the configuration
						context.Response.StatusCode = 400;
					} else if (result.Webhook == null) {
						context.Response.StatusCode = 400;
					} else if (asyncHandler != null) {
						await asyncHandler(context, result.Webhook, context.RequestAborted);
					} else if (syncHandler != null) {
						syncHandler(context, result.Webhook);
					} else {
						await next(context);
					}
				}
			} catch (Exception ex) {
				// TODO: log this error ...

				context.Response.StatusCode = 500;
				// TODO: should we emit anything here?
			}
		}
	}
}
