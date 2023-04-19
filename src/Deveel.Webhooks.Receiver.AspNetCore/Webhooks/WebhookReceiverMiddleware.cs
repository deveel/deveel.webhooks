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

namespace Deveel.Webhooks {
    class WebhookReceiverMiddleware<TWebhook> : IMiddleware where TWebhook : class {
		private readonly IEnumerable<IWebhookHandler<TWebhook>> handlers;
		private readonly IWebhookReceiver<TWebhook> receiver;

		public WebhookReceiverMiddleware(IWebhookReceiver<TWebhook> receiver, IEnumerable<IWebhookHandler<TWebhook>> handlers) {
			this.receiver = receiver;
			this.handlers = handlers;
		}

		public async Task InvokeAsync(HttpContext context, RequestDelegate next) {
			try {
				var result = await receiver.ReceiveAsync(context.Request, context.RequestAborted);
				
				if (result.SignatureValid != null && !result.SignatureValid.Value) {
					// TODO: get this from the configuration
					context.Response.StatusCode = 400;
				} else if (result.Webhook == null) {
					context.Response.StatusCode = 400;
				} else if (handlers != null) {
					foreach (var handler in handlers) {
						await handler.HandleAsync(result.Webhook, context.RequestAborted);
					}
				} else {
					await next(context);
				}
			} catch (Exception ex) {
				// TODO: log this error ...

				context.Response.StatusCode = 500;
				// TODO: should we emit anything here?
			}
		}
	}
}
