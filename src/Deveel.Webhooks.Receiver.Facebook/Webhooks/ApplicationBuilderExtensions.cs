using Deveel.Facebook;

using Microsoft.AspNetCore.Builder;

namespace Deveel.Webhooks {
	public static class ApplicationBuilderExtensions {
		public static IApplicationBuilder UseFacebookReceiver(this IApplicationBuilder app, string path = "/facebook", FacebookWebhookHandlingOptions? options = null) {
			app.UseWebhookReceiver<FacebookWebhook>(path, new WebhookHandlingOptions {
				ResponseStatusCode = 200,
				InvalidStatusCode = 400,
				MaxParallelThreads = options?.MaxParallelThreads ?? 1,
				ExecutionMode = options?.ExecutionMode
			});
			return app;
		}

		public static IApplicationBuilder UseFacebookReceiver(this IApplicationBuilder app, string path, Func<FacebookWebhook, Task> handler)
			=> app.UseWebhookReceiver<FacebookWebhook>(path, handler);

		public static IApplicationBuilder UseFacebookReceiver<T1>(this IApplicationBuilder app, string path, Func<FacebookWebhook, T1, Task> handler)
			=> app.UseWebhookReceiver<FacebookWebhook, T1>(path, handler);

		public static IApplicationBuilder UseFacebookReceiver<T1, T2>(this IApplicationBuilder app, string path, Func<FacebookWebhook, T1, T2, Task> handler)
			=> app.UseWebhookReceiver<FacebookWebhook, T1, T2>(path, handler);

		public static IApplicationBuilder UseFacebookReceiver<T1, T2, T3>(this IApplicationBuilder app, string path, Func<FacebookWebhook, T1, T2, T3, Task> handler)
			=> app.UseWebhookReceiver<FacebookWebhook, T1, T2, T3>(path, handler);

		public static IApplicationBuilder UseFacebookReceiver(this IApplicationBuilder app, string path, Action<FacebookWebhook> handler)
			=> app.UseWebhookReceiver<FacebookWebhook>(path, handler);

		public static IApplicationBuilder UseFacebookReceiver<T1>(this IApplicationBuilder app, string path, Action<FacebookWebhook, T1> handler)
			=> app.UseWebhookReceiver<FacebookWebhook, T1>(path, handler);

		public static IApplicationBuilder UseFacebookReceiver<T1, T2>(this IApplicationBuilder app, string path, Action<FacebookWebhook, T1, T2> handler)
			=> app.UseWebhookReceiver<FacebookWebhook, T1, T2>(path, handler);

		public static IApplicationBuilder UseFacebookReceiver<T1, T2, T3>(this IApplicationBuilder app, string path, Action<FacebookWebhook, T1, T2, T3> handler)
			=> app.UseWebhookReceiver<FacebookWebhook, T1, T2, T3>(path, handler);

		public static IApplicationBuilder UseFacebookVerifier(this IApplicationBuilder app, string path = "/facebook/verify")
			=> app.UseWebhookVerfier<FacebookWebhook>(path);
	}
}
