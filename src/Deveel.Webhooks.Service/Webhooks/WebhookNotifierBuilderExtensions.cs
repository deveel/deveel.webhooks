using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Webhooks {
	public static class WebhookNotifierBuilderExtensions {
		public static WebhookNotifierBuilder<TWebhook> UseDefaultSubscriptionResolver<TWebhook>(this WebhookNotifierBuilder<TWebhook> builder, Type subscriptionType)
			where TWebhook : class {
			if (!typeof(IWebhookSubscription).IsAssignableFrom(subscriptionType))
				throw new ArgumentException("The type specified is not a subscription type", nameof(subscriptionType));

			var resolverType = typeof(DefaultWebhookSubscriptionResolver<>).MakeGenericType(subscriptionType);
			return builder.UseSubscriptionResolver(resolverType, ServiceLifetime.Scoped);
		}
	}
}
