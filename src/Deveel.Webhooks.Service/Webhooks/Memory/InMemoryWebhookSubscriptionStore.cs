using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Data;
using Deveel.Webhooks;
using Deveel.Webhooks.Storage;

namespace Deveel.Webhooks.Memory {
	class InMemoryWebhookSubscriptionStore : InMemoryStore<InMemoryWebhookSubscription, IWebhookSubscription>, IWebhookSubscriptionStore {
		public InMemoryWebhookSubscriptionStore() {
		}

		public InMemoryWebhookSubscriptionStore(IStoreState<InMemoryWebhookSubscription> state) : base(state) {
		}

		public InMemoryWebhookSubscriptionStore(InMemoryOptions<InMemoryWebhookSubscription> options) : base(options) {
		}

		public InMemoryWebhookSubscriptionStore(InMemoryOptions<InMemoryWebhookSubscription> options, IStoreState<InMemoryWebhookSubscription> state) : base(options, state) {
		}

		public Task<IList<IWebhookSubscription>> GetByEventTypeAsync(string eventType, bool activeOnly, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			var entities = Entities.AsQueryable();
			if (Options.MultiTenancy != null &&
				Options.MultiTenancy.Handling == InMemoryMultiTenancyHandling.TenantField)
				entities = entities.Where(x => x.TenantId == Options.TenantId);

			var query = entities
				.Where(x => x.EventTypes.Any(x => x == eventType));

			if (activeOnly)
				query = query.Where(x => x.IsActive == true);

			var result = query
				.Cast<IWebhookSubscription>()
				.ToList();

			return Task.FromResult<IList<IWebhookSubscription>>(result);
		}

		public Task<PaginatedResult<IWebhookSubscription>> GetPageByMetadataAsync(string key, object value, PageRequest page, CancellationToken cancellationToken) {
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();

			var count = State.Count;
			var subscriptions = State.Values.Where(x => x.Metadata.Any(y => y.Key == key && y.Value == value));

			return Task.FromResult(new PaginatedResult<IWebhookSubscription>(page, count, subscriptions));
		}

		public Task<bool> MetadataExistsAsync(string key, object value, CancellationToken cancellationToken) {
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();

			var result = State.Values.Any(x => x.Metadata.Any(y => y.Key == key && y.Value == value));

			return Task.FromResult(result);
		}

		public Task SetStateAsync(IWebhookSubscription subscription, bool active, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			((InMemoryWebhookSubscription)subscription).IsActive = active;

			return Task.CompletedTask;
		}
	}
}
