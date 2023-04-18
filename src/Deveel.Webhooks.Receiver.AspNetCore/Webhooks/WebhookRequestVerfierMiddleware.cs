using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace Deveel.Webhooks {
	class WebhookRequestVerfierMiddleware<TWebhook> : IMiddleware where TWebhook : class {
		public Task InvokeAsync(HttpContext context, RequestDelegate next) => throw new NotImplementedException();
	}
}
