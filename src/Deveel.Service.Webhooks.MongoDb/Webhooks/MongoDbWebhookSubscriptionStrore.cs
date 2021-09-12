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

		public async Task<IList<WebhookSubscriptionDocument>> GetByEventTypeAsync(string eventType, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			var filter = Builders<WebhookSubscriptionDocument>.Filter
				.AnyEq(doc => doc.EventTypes, eventType);
			return await FindAllAsync(filter, cancellationToken: cancellationToken);
		}

		async Task<IList<IWebhookSubscription>> IWebhookSubscriptionStore<IWebhookSubscription>.GetByEventTypeAsync(string eventType, CancellationToken cancellationToken) {
			var result = await GetByEventTypeAsync(eventType, cancellationToken);
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
