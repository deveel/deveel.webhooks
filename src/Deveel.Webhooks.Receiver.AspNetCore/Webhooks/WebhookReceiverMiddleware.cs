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

		private async Task HandleWebhookAsync(TWebhook webhook, CancellationToken cancellationToken) {
			if (handlers == null)
				return;

			var mode = options.ExecutionMode ?? HandlerExecutionMode.Parallel;

			switch (mode) {
				case HandlerExecutionMode.Sequential:
					foreach (var handler in handlers) {
						await ExecuteAsync(handler, webhook, cancellationToken);
					}
					break;
				case HandlerExecutionMode.Parallel:
					var parallelOptions = new ParallelOptions {
						CancellationToken = cancellationToken,
						MaxDegreeOfParallelism = options.MaxParallelThreads ?? Environment.ProcessorCount
					};
					await Parallel.ForEachAsync(handlers, parallelOptions, async (handler, token) => {
						await ExecuteAsync(handler, webhook, token);
					});

					break;
			}
		}

		private async Task ExecuteAsync(IWebhookHandler<TWebhook> handler, TWebhook webhook, CancellationToken cancellationToken) {
			try {
				await handler.HandleAsync(webhook, cancellationToken);
			} catch (Exception ex) {
				logger.LogUnhandledHandlerError(ex, handler.GetType(), typeof(TWebhook));
			}
		}

		public async Task InvokeAsync(HttpContext context, RequestDelegate next) {
			try {
				logger.TraceWebhookArrived();

				var result = await receiver.ReceiveAsync(context.Request, context.RequestAborted);

				if (result.Successful) {
					logger.TraceWebhookReceived();
				} else if (result.SignatureFailed) {
					logger.WarnInvalidSignature();
				} else if (!result.Successful) {
					logger.WarnInvalidWebhook();
				}

				if (handlers != null && result.Successful && result.Webhook != null) {
					await HandleWebhookAsync(result.Webhook, context.RequestAborted);
				}

				await next.Invoke(context);

				if (!context.Response.HasStarted) {
					if (result.Successful) {
                        context.Response.StatusCode = SuccessStatusCode;
                    } else if (result.SignatureFailed) {
						context.Response.StatusCode = InvalidStatusCode;
					} else {
						context.Response.StatusCode = FailureStatusCode;
					}

					// TODO: should we emit anything here?
                    await context.Response.WriteAsync("");
				}
			} catch (WebhookReceiverException ex) {
				logger.LogUnhandledReceiveError(ex);

				if (!context.Response.HasStarted) {
					context.Response.StatusCode = FailureStatusCode;

					// TODO: should we emit anything here?
					await context.Response.WriteAsync("");
				}
			}

		}
	}
}
