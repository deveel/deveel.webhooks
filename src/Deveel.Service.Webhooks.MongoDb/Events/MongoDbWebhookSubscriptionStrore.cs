using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Data;
using Deveel.Webhooks;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

namespace Deveel.Events {
	class MongoDbWebhookSubscriptionStrore : MongoDbEntityStore<WebhookSubscriptionDocument, IWebhookSubscription>,
													IWebhookSubscriptionStore {
		public MongoDbWebhookSubscriptionStrore(IOptions<MongoDbOptions<WebhookSubscriptionDocument>> options) : base(options) {
		}

		public MongoDbWebhookSubscriptionStrore(MongoDbOptions<WebhookSubscriptionDocument> options) : base(options) {
		}

		public async Task<IList<WebhookSubscriptionDocument>> FilterAsync(IEvent @event, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			var subscriptions = await FindAllAsync(x => x.EventType == @event.Type, cancellationToken);

			return subscriptions.Where(x => x.Matches(@event)).ToList();
		}

		public Task<PaginatedResult<WebhookSubscriptionDocument>> GetPageByMetadataAsync(string key, object value, PageRequest page, CancellationToken cancellationToken) {
			return GetPageAsync(Builders<WebhookSubscriptionDocument>.Filter.ElemMatch(doc => doc.Metadata, item => item.Key == key && item.Value == value), page, cancellationToken);
		}

		public Task<bool> MetadataExistsAsync(string key, object value, CancellationToken cancellationToken)
			=> ExistsAsync(Builders<WebhookSubscriptionDocument>.Filter.ElemMatch(doc => doc.Metadata, item => item.Key == key && item.Value == value), cancellationToken);

		public Task SetStateAsync(WebhookSubscriptionDocument subscription, bool active, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			subscription.IsActive = active;

			return Task.CompletedTask;
		}

		async Task<PaginatedResult<IWebhookSubscription>> IWebhookSubscriptionStore.GetPageByMetadataAsync(string key, object value, PageRequest page, CancellationToken cancellationToken) {
			var result = await GetPageByMetadataAsync(key, value, page, cancellationToken);
			return result.CastTo<IWebhookSubscription>();
		}

		Task IWebhookSubscriptionStore.SetStateAsync(IWebhookSubscription subscription, bool active, CancellationToken cancellationToken) 
			=>  SetStateAsync(Assert(subscription), active, cancellationToken);
	}
}
