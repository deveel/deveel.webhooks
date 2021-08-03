﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Data;
using Deveel.Data.Memory;
using Deveel.Webhooks;

namespace Deveel.Webhooks.Memory {
	class InMemoryWebhookSubscriptionStore : InMemoryStore<InMemoryWebhookSubscription, IWebhookSubscription>, IWebhookSubscriptionStore {
		public InMemoryWebhookSubscriptionStore() {
		}

		public InMemoryWebhookSubscriptionStore(IStoreState<InMemoryWebhookSubscription> state) : base(state) {
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