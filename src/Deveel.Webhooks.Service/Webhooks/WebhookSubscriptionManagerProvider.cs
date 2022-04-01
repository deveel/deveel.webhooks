using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Deveel.Webhooks {
	public class WebhookSubscriptionManagerProvider<TSubscription> : IWebhookSubscriptionManagerProvider<TSubscription>, IDisposable where TSubscription : class, IWebhookSubscription {
		private readonly IWebhookSubscriptionStoreProvider<TSubscription> storeProvider;
		private readonly ILoggerFactory loggerFactory;
		private readonly IWebhookSubscriptionFactory<TSubscription> subscriptionFactory;
		private List<IWebhookSubscriptionStore<TSubscription>> stores;

		public WebhookSubscriptionManagerProvider(IWebhookSubscriptionStoreProvider<TSubscription> storeProvider, IWebhookSubscriptionFactory<TSubscription> subscriptionFactory, ILoggerFactory loggerFactory) {
			this.storeProvider = storeProvider;
			this.subscriptionFactory = subscriptionFactory;
			this.loggerFactory = loggerFactory;
		}

		public WebhookSubscriptionManagerProvider(IWebhookSubscriptionStoreProvider<TSubscription> storeProvider, IWebhookSubscriptionFactory<TSubscription> subscriptionFactory)
			: this(storeProvider, subscriptionFactory, NullLoggerFactory.Instance) {
		}


		public IWebhookSubscriptionManager<TSubscription> GetManager(string tenantId) {
			var store = storeProvider.GetTenantStore(tenantId);

			if (stores == null)
				stores = new List<IWebhookSubscriptionStore<TSubscription>>();

			stores.Add(store);

			return CreateUserManager(store);
		}

		protected virtual IWebhookSubscriptionManager<TSubscription> CreateUserManager(IWebhookSubscriptionStore<TSubscription> store) {
			var logger = loggerFactory.CreateLogger<WebhookSubscriptionManager<TSubscription>>();
			return new WebhookSubscriptionManager<TSubscription>(store, subscriptionFactory, logger);
		}

		public void Dispose() {
			if (stores != null) {
				foreach (var store in stores) {
					store?.Dispose();
				}
			}
		}
	}
}
