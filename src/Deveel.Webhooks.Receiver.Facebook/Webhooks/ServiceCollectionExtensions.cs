using Deveel.Facebook;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	public static class ServiceCollectionExtensions {
		public static WebhookReceiverBuilder<FacebookWebhook> AddFacebookReceiver(this IServiceCollection services) {
			var builder = services.AddWebhookReceiver<FacebookWebhook>()
				.Configure(_ => { });

			services.AddWebhookVerifier<FacebookWebhook>()
				.UseVerifier<FacebookRequestVerifier>();

			services.AddTransient<IPostConfigureOptions<WebhookReceiverOptions<FacebookWebhook>>, ConfigureWebhookReceiverOptions>();
			services.AddTransient<IPostConfigureOptions<WebhookHandlingOptions>, ConfigureWebhookHandlingOptions>();
			services.AddTransient<IPostConfigureOptions<WebhookVerificationOptions<FacebookWebhook>>, ConfigureWebhookVerificationOptions>();

			return builder;
		}

		public static WebhookReceiverBuilder<FacebookWebhook> AddFacebookReceiver(this IServiceCollection services, string sectionPath) {
			services.AddOptions<FacebookReceiverOptions>()
				.BindConfiguration(sectionPath);

			return services.AddFacebookReceiver();
		}

		public static WebhookReceiverBuilder<FacebookWebhook> AddFacebookReceiver(this IServiceCollection services, Action<FacebookReceiverOptions> configure) {
			services.Configure(configure);

			return services.AddFacebookReceiver();
		}
	}
}
