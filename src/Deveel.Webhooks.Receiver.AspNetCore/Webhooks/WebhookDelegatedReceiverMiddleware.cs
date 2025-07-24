// Copyright 2022-2025 Antonello Provenzano
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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Deveel.Webhooks {
    class WebhookDelegatedReceiverMiddleware<TWebhook> where TWebhook : class {
		private readonly RequestDelegate next;
		private readonly IWebhookHandler<TWebhook> webhookHandler;
		private readonly WebhookHandlingOptions options;
		private readonly ILogger logger;

		public WebhookDelegatedReceiverMiddleware(
			RequestDelegate next,
            WebhookHandlingOptions options ,
            IWebhookHandler<TWebhook> webhookHandler,
			ILogger<WebhookDelegatedReceiverMiddleware<TWebhook>>? logger = null) {
			this.next = next;
			this.options = options;
			this.logger = logger ?? NullLogger<WebhookDelegatedReceiverMiddleware<TWebhook>>.Instance;
			this.webhookHandler = webhookHandler;
		}

		//public WebhookDelegatedReceiverMiddleware(
		//	RequestDelegate next,
		//	WebhookHandlingOptions options,
		//	Action<HttpContext, TWebhook> handler,
		//	ILogger<WebhookDelegatedReceiverMiddleware<TWebhook>>? logger = null) {
		//	this.next = next;
		//	this.options = options;
		//	this.logger = logger ?? NullLogger<WebhookDelegatedReceiverMiddleware<TWebhook>>.Instance;
		//	syncHandler = handler;
		//}

		public async Task InvokeAsync(HttpContext context) {
            try {
				logger.TraceWebhookArrived();

				var receiver = context.RequestServices.GetService<IWebhookReceiver<TWebhook>>();
				
				WebhookReceiveResult<TWebhook>? result = null;

				if (receiver != null) {
					result = await receiver.ReceiveAsync(context.Request, context.RequestAborted);

					if (result?.Successful ?? false) {
						var webhooks = result?.Webhooks;

						if (webhooks == null || webhooks.Count == 0) {
							logger.WarnInvalidWebhook();
						} else {
							logger.TraceWebhookReceived();

							if (webhookHandler is IWebhookHandlerInitialize<TWebhook> init)
								init.Initialize(context.RequestServices);

							foreach (var webhook in webhooks) {
								await webhookHandler.HandleAsync(webhook, context.RequestAborted);
							}

							logger.TraceWebhookHandled(webhookHandler.GetType());
						}
					} else {
						logger.WarnInvalidWebhook();
					}
				} else if (result?.SignatureFailed ?? false) {
					logger.WarnInvalidSignature();
				} else {
					logger.WarnReceiverNotRegistered();
				}

				await next.Invoke(context);

				if (!context.Response.HasStarted && result != null) {
					if ((result?.SignatureValidated ?? false) && !(result?.SignatureValid ?? false)) {
						context.Response.StatusCode = options?.InvalidStatusCode ?? 400;
					} else if ((result?.Successful ?? false)) {
						context.Response.StatusCode = options?.ResponseStatusCode ?? 204;
					}

					// TODO: should we emit anything here?
					await context.Response.WriteAsync("");
				}
			} catch (WebhookReceiverException ex) {
				logger.LogUnhandledReceiveError(ex);

				if (!context.Response.HasStarted) {
					context.Response.StatusCode = options?.ErrorStatusCode ?? 500;
					// TODO: should we emit anything here?
					await context.Response.WriteAsync("");
				}
			}
		}
    }
}