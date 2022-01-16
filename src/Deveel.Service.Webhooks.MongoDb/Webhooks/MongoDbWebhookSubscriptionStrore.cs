using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Webhooks.Storage;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

namespace Deveel.Webhooks {
	class MongoDbWebhookSubscriptionStrore : MongoDbWebhookStoreBase<WebhookSubscriptionDocument, IWebhookSubscription>,
													IWebhookSubscriptionStore {
		public MongoDbWebhookSubscriptionStrore(IOptions<MongoDbWebhookOptions> options) : base(options) {
		}

		public MongoDbWebhookSubscriptionStrore(MongoDbWebhookOptions options) : base(options) {
		}

		protected override IMongoCollection<WebhookSubscriptionDocument> Collection => GetCollection(Options.SubscriptionsCollectionName);

		async Task<IWebhookSubscription> IWebhookSubscriptionStore<IWebhookSubscription>.GetByIdAsync(string id, CancellationToken cancellationToken)
			=> await base.GetByIdAsync(id, cancellationToken);

		public async Task<IList<WebhookSubscriptionDocument>> GetByEventTypeAsync(string eventType, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			var filter = Builders<WebhookSubscriptionDocument>.Filter
				.AnyEq(doc => doc.EventTypes, eventType);

			var result = await Collection.FindAsync(filter, cancellationToken: cancellationToken);
			return await result.ToListAsync(cancellationToken);
		}

		async Task<IList<IWebhookSubscription>> IWebhookSubscriptionStore<IWebhookSubscription>.GetByEventTypeAsync(string eventType,  bool activeOnly, CancellationToken cancellationToken) {
			var result = await GetByEventTypeAsync(eventType, activeOnly, cancellationToken);
			return result.Cast<IWebhookSubscription>().ToList();
		}

		public Task SetStateAsync(WebhookSubscriptionDocument subscription, WebhookSubscriptionStateInfo stateInfo, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			subscription.Status = stateInfo.Status;
			subscription.LastStatusTime = stateInfo.TimeStamp;

			return Task.CompletedTask;
		}

		Task IWebhookSubscriptionStore<IWebhookSubscription>.SetStateAsync(IWebhookSubscription subscription, WebhookSubscriptionStateInfo stateInfo, CancellationToken cancellationToken)
			=> SetStateAsync((WebhookSubscriptionDocument)subscription, stateInfo, cancellationToken);
	}
}
