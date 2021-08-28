using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Data;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

namespace Deveel.Webhooks {
	class MongoDbWebhookSubscriptionStrore : MongoDbEntityStore<WebhookSubscriptionDocument, IWebhookSubscription>,
													IWebhookSubscriptionStore {
		public MongoDbWebhookSubscriptionStrore(IOptions<MongoDbOptions<WebhookSubscriptionDocument>> options) : base(options) {
		}

		public MongoDbWebhookSubscriptionStrore(MongoDbOptions<WebhookSubscriptionDocument> options) : base(options) {
		}

		public async Task<IList<WebhookSubscriptionDocument>> GetByEventTypeAsync(string eventType, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			var filter = Builders<WebhookSubscriptionDocument>.Filter
				.AnyEq(doc => doc.EventTypes, eventType);
			var result = await Collection.FindAsync(filter, cancellationToken: cancellationToken);

			return await result.ToListAsync();
		}

		async Task<IList<IWebhookSubscription>> IWebhookSubscriptionStore<IWebhookSubscription>.GetByEventTypeAsync(string eventType, CancellationToken cancellationToken) {
			var result = await GetByEventTypeAsync(eventType, cancellationToken);
			return result.Cast<IWebhookSubscription>().ToList();
		}


		public Task<PaginatedResult<WebhookSubscriptionDocument>> GetPageByMetadataAsync(string key, object value, PageRequest page, CancellationToken cancellationToken) {
			var filter = Builders<WebhookSubscriptionDocument>.Filter
				.ElemMatch(doc => doc.Metadata, item => item.Key == key && item.Value == value);
			return GetPageAsync(filter, page, cancellationToken);
		}

		public Task<bool> MetadataExistsAsync(string key, object value, CancellationToken cancellationToken)
			=> ExistsAsync(Builders<WebhookSubscriptionDocument>.Filter.ElemMatch(doc => doc.Metadata, item => item.Key == key && item.Value == value), cancellationToken);

		public Task SetStateAsync(WebhookSubscriptionDocument subscription, bool active, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			subscription.IsActive = active;

			return Task.CompletedTask;
		}

		async Task<PaginatedResult<IWebhookSubscription>> IWebhookSubscriptionStore<IWebhookSubscription>.GetPageByMetadataAsync(string key, object value, PageRequest page, CancellationToken cancellationToken) {
			var result = await GetPageByMetadataAsync(key, value, page, cancellationToken);
			return result.CastTo<IWebhookSubscription>();
		}

		Task IWebhookSubscriptionStore<IWebhookSubscription>.SetStateAsync(IWebhookSubscription subscription, bool active, CancellationToken cancellationToken)
			=> SetStateAsync(Assert(subscription), active, cancellationToken);
	}
}
