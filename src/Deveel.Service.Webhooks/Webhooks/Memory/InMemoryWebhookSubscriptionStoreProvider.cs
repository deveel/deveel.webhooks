using System;

using Deveel.Data.Memory;
using Deveel.Webhooks;

namespace Deveel.Webhooks.Memory {
	class InMemoryWebhookSubscriptionStoreProvider : InMemoryStoreProvider<InMemoryWebhookSubscription, IWebhookSubscription>, IWebhookSubscriptionStoreProvider {
		public InMemoryWebhookSubscriptionStoreProvider() {
		}

		public InMemoryWebhookSubscriptionStoreProvider(IStoreProviderState<InMemoryWebhookSubscription> state) : base(state) {
		}

		protected override InMemoryStore<InMemoryWebhookSubscription> CreateStore(IStoreState<InMemoryWebhookSubscription> state) 
			=> new InMemoryWebhookSubscriptionStore(state);

		IWebhookSubscriptionStore IWebhookSubscriptionStoreProvider.GetStore(string tenantId) 
			=> (IWebhookSubscriptionStore)GetStore(tenantId);

		IWebhookSubscriptionStore<IWebhookSubscription> IWebhookSubscriptionStoreProvider<IWebhookSubscription>.GetStore(string tenantId)
			=> (IWebhookSubscriptionStore<IWebhookSubscription>)GetStore(tenantId);
	}
}
