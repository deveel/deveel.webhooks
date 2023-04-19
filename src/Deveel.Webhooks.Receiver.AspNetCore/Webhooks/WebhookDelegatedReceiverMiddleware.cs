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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

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

		private WebhookReceiverOptions GetOptions(HttpContext context) {
			var snapshot = context?.RequestServices?.GetService<IOptionsSnapshot<WebhookReceiverOptions>>();
			return snapshot?.GetReceiverOptions<TWebhook>() ?? new WebhookReceiverOptions();
		}

		private ILogger GetLogger(HttpContext context) {
			var loggerFactory = context?.RequestServices?.GetService<ILoggerFactory>();
            return loggerFactory?.CreateLogger<WebhookDelegatedReceiverMiddleware<TWebhook>>() ?? 
				NullLogger<WebhookDelegatedReceiverMiddleware<TWebhook>>.Instance;
		}


		public async Task InvokeAsync(HttpContext context) {
            var options = GetOptions(context);
			var logger = GetLogger(context);

            try {
				logger.TraceWebhookArrived();

				var receiver = context.RequestServices.GetService<IWebhookReceiver<TWebhook>>();
				
				WebhookReceiveResult<TWebhook>? result = null;

				if (receiver != null) {
					result = await receiver.ReceiveAsync(context.Request, context.RequestAborted);

					if (result?.Successful ?? false) {
						var webhook = result?.Webhook;

						if (webhook == null) {
							logger.WarnInvalidWebhook();
						} else {
							logger.TraceWebhookReceived();

							if (asyncHandler != null) {
								await asyncHandler(context, webhook, context.RequestAborted);

								logger.TraceWebhookHandled(typeof(Func<TWebhook, CancellationToken, Task>));
							} else if (syncHandler != null) {
								syncHandler(context, webhook);

								logger.TraceWebhookHandled(typeof(Action<TWebhook>));
							}
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