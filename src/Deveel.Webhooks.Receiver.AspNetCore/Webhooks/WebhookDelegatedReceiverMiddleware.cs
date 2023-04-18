using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace Deveel.Webhooks {
	class WebhookDelegatedReceiverMiddleware<TWerbhook> {
		private readonly RequestDelegate next;
		private readonly Func<HttpContext, TWerbhook, CancellationToken, Task> receiver;

		public WebhookDelegatedReceiverMiddleware(RequestDelegate next, Func<HttpContext, TWerbhook, CancellationToken, Task> receiver) {
			this.next = next;
			this.receiver = receiver;
		}

		public Task InvokeAsync(HttpContext context) {
			return Task.CompletedTask;
		}
	}
}
