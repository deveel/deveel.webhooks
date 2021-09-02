using System;

using Deveel.Data.Memory;
using Deveel.Webhooks;

namespace Deveel.Webhooks.Memory {
	class InMemoryWebhookSubscriptionStoreProvider : InMemoryStoreProvider<InMemoryWebhookSubscription, IWebhookSubscription>, IWebhookSubscriptionStoreProvider {
		public InMemoryWebhookSubscriptionStoreProvider() {
		}

		public InMemoryWebhookSubscriptionStoreProvider(IStoreProviderState<InMemoryWebhookSubscription> state) : base(state) {
		}

		public InMemoryWebhookSubscriptionStoreProvider(IStoreState<InMemoryWebhookSubscription> state) : base(state) {
		}

		public InMemoryWebhookSubscriptionStoreProvider(InMemoryOptions<InMemoryWebhookSubscription> options) : base(options) {
		}

		public InMemoryWebhookSubscriptionStoreProvider(InMemoryOptions<InMemoryWebhookSubscription> options, IStoreState<InMemoryWebhookSubscription> state) : base(options, state) {
		}

		public InMemoryWebhookSubscriptionStoreProvider(InMemoryOptions<InMemoryWebhookSubscription> options, IStoreProviderState<InMemoryWebhookSubscription> state) : base(options, state) {
		}

		protected override InMemoryStore<InMemoryWebhookSubscription> CreateStore(InMemoryOptions<InMemoryWebhookSubscription> options, IStoreState<InMemoryWebhookSubscription> state) 
			=> new InMemoryWebhookSubscriptionStore(options, state);

		IWebhookSubscriptionStore IWebhookSubscriptionStoreProvider.GetStore(string tenantId) 
			=> (IWebhookSubscriptionStore)GetStore(tenantId);

		IWebhookSubscriptionStore<IWebhookSubscription> IWebhookSubscriptionStoreProvider<IWebhookSubscription>.GetStore(string tenantId)
			=> (IWebhookSubscriptionStore<IWebhookSubscription>)GetStore(tenantId);
	}
}
