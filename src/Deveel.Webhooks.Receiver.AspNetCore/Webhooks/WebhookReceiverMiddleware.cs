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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	class WebhookReceiverMiddleware<TWebhook> : IMiddleware where TWebhook : class {
		private readonly IEnumerable<IWebhookHandler<TWebhook>>? handlers;
		private readonly IWebhookReceiver<TWebhook> receiver;
		private readonly WebhookReceiverOptions options;
		private readonly ILogger logger;

		public WebhookReceiverMiddleware(
			IOptionsSnapshot<WebhookReceiverOptions> options,
			IWebhookReceiver<TWebhook> receiver,
			IEnumerable<IWebhookHandler<TWebhook>>? handlers = null,
			ILogger<WebhookReceiverMiddleware<TWebhook>>? logger = null) {
			this.options = options.GetReceiverOptions<TWebhook>();
			this.receiver = receiver;
			this.handlers = handlers;
			this.logger = logger ?? NullLogger<WebhookReceiverMiddleware<TWebhook>>.Instance;
		}

		private int SuccessStatusCode => options.ResponseStatusCode ?? 200;

		private int FailureStatusCode => options.ErrorStatusCode ?? 500;

		private int InvalidStatusCode => options.InvalidStatusCode ?? 400;

		public async Task InvokeAsync(HttpContext context, RequestDelegate next) {
			try {
				var result = await receiver.ReceiveAsync(context.Request, context.RequestAborted);
				
				if (handlers != null && result.Successful && result.Webhook != null) {
					foreach (var handler in handlers) {
						await handler.HandleAsync(result.Webhook, context.RequestAborted);
					}
				}

				await next.Invoke(context);

				if (!context.Response.HasStarted) {
					if (result.Successful) {
                        context.Response.StatusCode = SuccessStatusCode;
                    } else if (result.SignatureValidated && !result.SignatureValid.Value) {
						context.Response.StatusCode = InvalidStatusCode;
					} else {
						context.Response.StatusCode = FailureStatusCode;
					}

					// TODO: should we emit anything here?
                    await context.Response.WriteAsync("");
				}
			} catch (WebhookReceiverException ex) {
				// TODO: log this error ...

				if (!context.Response.HasStarted) {
					context.Response.StatusCode = FailureStatusCode;

					// TODO: should we emit anything here?
					await context.Response.WriteAsync("");
				}

				// TODO: should we emit anything here?
			}

		}
	}
}
