using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Webhooks {
	public static class ServiceCollectionExtensions {
		public static IServiceCollection AddCallback<TWebhook>(this IServiceCollection services, IWebhookCallback<TWebhook> callback)
			where TWebhook : class
			=> services.AddSingleton(callback);

		public static IServiceCollection AddCallback<TWebhook>(this IServiceCollection services, Action<TWebhook?> callback)
			where TWebhook : class
			=> services.AddSingleton<IWebhookCallback<TWebhook>>(WebhookCallback.Create(callback));

	}
}
