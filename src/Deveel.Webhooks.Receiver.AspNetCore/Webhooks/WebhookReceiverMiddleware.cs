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

using System.Text;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Deveel.Webhooks {
	/// <summary>
	/// A middleware that handles the incoming webhooks through
	/// HTTP requests sent to a specific path in a web application.
	/// </summary>
	/// <typeparam name="TWebhook">
	/// The type of webhooks that are handled by this middleware
	/// </typeparam>
	public class WebhookReceiverMiddleware<TWebhook> where TWebhook : class {
		private readonly RequestDelegate next;
        private readonly WebhookHandlingOptions options;
        private readonly ILogger logger;

		/// <summary>
		/// Constructs a new instance of the middleware
		/// </summary>
		/// <param name="next">
		/// The next middleware in the pipeline of the web application
		/// </param>
		/// <param name="options">
		/// An optional set of options to configure the middleware behaviors
		/// </param>
		/// <param name="logger">
		/// A logger to use for tracing the execution of the middleware
		/// </param>
		public WebhookReceiverMiddleware(
			RequestDelegate next,
			WebhookHandlingOptions? options = null,
			ILogger<WebhookReceiverMiddleware<TWebhook>>? logger = null) {
            this.next = next;
            this.options = options ?? new WebhookHandlingOptions();
            this.logger = logger ?? NullLogger<WebhookReceiverMiddleware<TWebhook>>.Instance;
		}

		private int SuccessStatusCode => options.ResponseStatusCode ?? 200;

		private int FailureStatusCode => options.ErrorStatusCode ?? 500;

		private int InvalidStatusCode => options.InvalidStatusCode ?? 400;

		private async Task HandleWebhookResultAsync(IEnumerable<IWebhookHandler<TWebhook>> handlers, WebhookReceiveResult<TWebhook> result, CancellationToken cancellationToken) {
			if (handlers == null)
				return;

			var mode = options.ExecutionMode ?? HandlerExecutionMode.Parallel;

			if (result.Webhooks == null)
				return;

			switch (mode) {
				case HandlerExecutionMode.Sequential:
					foreach (var webhook in result.Webhooks) {
						foreach (var handler in handlers) {
							await ExecuteSequentialAsync(handler, webhook, cancellationToken);
						}
					}
					break;
				case HandlerExecutionMode.Parallel: {
					var parallelOptions = new ParallelOptions {
						CancellationToken = cancellationToken,
						MaxDegreeOfParallelism = options.MaxParallelThreads ?? Environment.ProcessorCount
					};
					await Parallel.ForEachAsync(handlers, parallelOptions, async (handler, token) => {
						foreach (var webhook in result.Webhooks) {
							await ExecuteParallelAsync(handler, webhook, token);
						}
					});

					break;
				}
			}
		}

        private async Task ExecuteParallelAsync(IWebhookHandler<TWebhook> handler, TWebhook webhook, CancellationToken cancellationToken) {
            try {
                await HandleWebhookAsync(handler, webhook, cancellationToken);
            } catch (Exception ex) {
                logger.LogUnhandledHandlerError(ex, handler.GetType(), typeof(TWebhook));
            }
        }


        private async Task ExecuteSequentialAsync(IWebhookHandler<TWebhook> handler, TWebhook webhook, CancellationToken cancellationToken) {
			try {
				await HandleWebhookAsync(handler, webhook, cancellationToken);
			} catch (Exception ex) {
				logger.LogUnhandledHandlerError(ex, handler.GetType(), typeof(TWebhook));
				throw new WebhookReceiverException($"Error while executing the handler '{handler.GetType()}'", ex);
			}
		}

		/// <summary>
		/// Receives the webhook from the HTTP request context given.
		/// </summary>
		/// <param name="context">
		/// The HTTP context of the request that contains the webhook
		/// to be received.
		/// </param>
		/// <returns>
		/// Returns the result of the webhook reception operation.
		/// </returns>
		protected virtual Task<WebhookReceiveResult<TWebhook>> ReceiveWebhookAsync(HttpContext context) {
            var receiver = context.RequestServices.GetRequiredService<IWebhookReceiver<TWebhook>>();

			return receiver.ReceiveAsync(context.Request, context.RequestAborted);
        }

		/// <summary>
		/// Resolves the handlers that are registered in the service container
		/// of the application.
		/// </summary>
		/// <param name="context">
		/// The HTTP context that is executed by the middleware.
		/// </param>
		/// <returns>
		/// Returns a sequence of handlers that are registered in the service
		/// container of the application.
		/// </returns>
		protected virtual IEnumerable<IWebhookHandler<TWebhook>> ResolveHandlers(HttpContext context) {
			var handlers = context.RequestServices.GetServices<IWebhookHandler<TWebhook>>();

			var init = handlers.OfType<IWebhookHandlerInitialize<TWebhook>>();
			if (init != null) {
				foreach (var handler in init) {
					handler.Initialize(context.RequestServices);
				}
			}

			return handlers;
		}

		/// <summary>
		/// Executes the given handler for the given webhook.
		/// </summary>
		/// <param name="handler">
		/// The instance of the handler to be executed.
		/// </param>
		/// <param name="webhook">
		/// The instance of the webhook to be handled.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token to cancel the execution of the handler.
		/// </param>
		/// <returns>
		/// Returns a task that completes when the handler has been executed.
		/// </returns>
		protected virtual Task HandleWebhookAsync(IWebhookHandler<TWebhook> handler, TWebhook webhook, CancellationToken cancellationToken) {
            return handler.HandleAsync(webhook, cancellationToken);
        }

		/// <summary>
		/// Handles the response of the webhook receiver middleware.
		/// </summary>
		/// <param name="context">
		/// The HTTP context of the request that contains the webhook
		/// </param>
		/// <param name="result">
		/// The result of the webhook reception operation.
		/// </param>
		/// <returns>
		/// Returns a task that completes when the response has been handled.
		/// </returns>
		protected virtual async Task OnWebhookHandledAsync(HttpContext context, WebhookReceiveResult<TWebhook> result) {
			if (result.Successful) {
				context.Response.StatusCode = SuccessStatusCode;

				// Handle more complex objects as result ...
				context.Response.ContentType = "text/plain";

				if (SuccessStatusCode == 200) {
					await context.Response.WriteAsync("OK", Encoding.UTF8, context.RequestAborted);
				}
			} else if (result.SignatureFailed) {
				context.Response.StatusCode = InvalidStatusCode;

				if (InvalidStatusCode == 400) {
					await context.Response.WriteAsync("Invalid signature", Encoding.UTF8, context.RequestAborted);
				}
			} else if (!result.Successful) {
				context.Response.StatusCode = FailureStatusCode;

				if (FailureStatusCode == 500) {
					await context.Response.WriteAsync("Invalid webhook", Encoding.UTF8, context.RequestAborted);
				}
			}
		}

		/// <summary>
		/// The main entry point of the middleware.
		/// </summary>
		/// <param name="context">
		/// The HTTP context of the request that is executed by the middleware.
		/// </param>
		/// <returns>
		/// Returns a task that completes when the middleware has finished
		/// </returns>
		public virtual async Task InvokeAsync(HttpContext context) {
			try {
				logger.TraceWebhookArrived();

				var result = await ReceiveWebhookAsync(context);

				if (result.Successful) {
					logger.TraceWebhookReceived();
				} else if (result.SignatureFailed) {
					logger.WarnInvalidSignature();
				} else if (!result.Successful) {
					logger.WarnInvalidWebhook();
				}

				if (result.Successful) {
					var handlers = ResolveHandlers(context);

					if (handlers != null) {
						await HandleWebhookResultAsync(handlers, result, context.RequestAborted);
					}
				}

				await next.Invoke(context);

				if (!context.Response.HasStarted) {
					await OnWebhookHandledAsync(context, result);
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
