using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public static class WebhookSubscriptionStoreExtensions {
		public static Task<string> CreateAsync<TSubscription>(this IWebhookSubscriptionStore<TSubscription> store, TSubscription entity)
			where TSubscription : class, IWebhookSubscription
			=> store.CreateAsync(entity, default);

		public static Task<TSubscription> FindByIdAsync<TSubscription>(this IWebhookSubscriptionStore<TSubscription> store, string id)
			where TSubscription : class, IWebhookSubscription
			=> store.FindByIdAsync(id, default);
	}
}
