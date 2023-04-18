using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Deveel.Webhooks {
	public static class ApplicationBuilderExtensions {
		public static IApplicationBuilder UseWebhookReceiver<TWebhook>(this IApplicationBuilder app, string path)
			where TWebhook : class {
			return app.MapWhen(
				context => context.Request.Method == "POST" && context.Request.Path.Equals(path),
				builder => builder.UseMiddleware<WebhookRceiverMiddleware<TWebhook>>()
			);
		}

		public static IApplicationBuilder UseWebhookVerifier<TWebhook>(this IApplicationBuilder app, string method, string path)
			where TWebhook : class
			=> app.MapWhen(
				context => context.Request.Method == method && context.Request.Path.Equals(path),
				builder => builder.UseMiddleware<WebhookRequestVerfierMiddleware<TWebhook>>()
			);

		public static IApplicationBuilder UseWebhookVerfier<TWebhook>(this IApplicationBuilder app, string path)
			where TWebhook : class
			=> app.UseWebhookVerifier<TWebhook>("GET", path);

		public static IApplicationBuilder UseWebhookReceiver<TWebhook>(this IApplicationBuilder app, string path, Func<HttpContext, TWebhook, CancellationToken, Task> receiver)
			where TWebhook : class {
			return app.MapWhen(
				context => context.Request.Method == "POST" && context.Request.Path.Equals(path),
				builder => builder.UseMiddleware<WebhookDelegatedReceiverMiddleware<TWebhook>>(receiver)
			);
		}
	}
}
