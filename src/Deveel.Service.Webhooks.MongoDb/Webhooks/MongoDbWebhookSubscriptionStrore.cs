using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Data;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

namespace Deveel.Webhooks {
	class MongoDbWebhookSubscriptionStrore : MongoDbStore<WebhookSubscriptionDocument, IWebhookSubscription>,
													IWebhookSubscriptionStore {
		public MongoDbWebhookSubscriptionStrore(IOptions<MongoDbOptions<WebhookSubscriptionDocument>> options) : base(options) {
		}

		public MongoDbWebhookSubscriptionStrore(MongoDbOptions<WebhookSubscriptionDocument> options) : base(options) {
		}

		public async Task<IList<WebhookSubscriptionDocument>> GetByEventTypeAsync(string eventType, bool activeOnly, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			var filter = Builders<WebhookSubscriptionDocument>.Filter
				.AnyEq(doc => doc.EventTypes, eventType);
			if (activeOnly) {
				var activeFilter = Builders<WebhookSubscriptionDocument>.Filter.Eq(x => x.IsActive, true);
				filter = Builders<WebhookSubscriptionDocument>.Filter.And(filter, activeFilter);
			}

			return await FindAllAsync(filter, cancellationToken: cancellationToken);
		}

		async Task<IList<IWebhookSubscription>> IWebhookSubscriptionStore<IWebhookSubscription>.GetByEventTypeAsync(string eventType,  bool activeOnly, CancellationToken cancellationToken) {
			var result = await GetByEventTypeAsync(eventType, activeOnly, cancellationToken);
			return result.Cast<IWebhookSubscription>().ToList();
		}

		public Task SetStateAsync(WebhookSubscriptionDocument subscription, bool active, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			subscription.IsActive = active;

			return Task.CompletedTask;
		}

		Task IWebhookSubscriptionStore<IWebhookSubscription>.SetStateAsync(IWebhookSubscription subscription, bool active, CancellationToken cancellationToken)
			=> SetStateAsync(Assert(subscription), active, cancellationToken);
	}
}
