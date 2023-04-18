using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Webhooks {
	class WebhookDelegatedReceiverMiddleware<TWebhook> where TWebhook : class {
		private readonly RequestDelegate next;
		private readonly Func<HttpContext, TWebhook, CancellationToken, Task> asyncHandler;
		private readonly Action<HttpContext, TWebhook> syncHandler;

		public WebhookDelegatedReceiverMiddleware(RequestDelegate next,
			Func<HttpContext, TWebhook, CancellationToken, Task> handler) {
			this.next = next;
			this.asyncHandler = handler;
		}

		public WebhookDelegatedReceiverMiddleware(RequestDelegate next,
			Action<HttpContext, TWebhook> handler) {
			this.next = next;
			this.syncHandler = handler;
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
