using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace Deveel.Webhooks {
	class WebhookRceiverMiddleware<TWebhook> : IMiddleware where TWebhook : class {
		private readonly IEnumerable<IWebhookHandler<TWebhook>> handlers;
		private readonly IWebhookReceiver<TWebhook> receiver;

		public WebhookRceiverMiddleware(IWebhookReceiver<TWebhook> receiver, IEnumerable<IWebhookHandler<TWebhook>> handlers) {
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
