// Copyright 2022 Deveel
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Webhooks.Storage;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

namespace Deveel.Webhooks.Storage {
	class MongoDbWebhookSubscriptionStrore : MongoDbWebhookStoreBase<WebhookSubscriptionDocument, IWebhookSubscription>,
											 IWebhookSubscriptionStore,
											 IWebhookSubscriptionPaginatedStore<IWebhookSubscription>,
											 IWebhookSubscriptionPaginatedStore<WebhookSubscriptionDocument>,
											 IWebhookSubscriptionQueryableStore<IWebhookSubscription>,
											 IWebhookSubscriptionQueryableStore<WebhookSubscriptionDocument> {
		public MongoDbWebhookSubscriptionStrore(IOptions<MongoDbWebhookOptions> options) : base(options) {
		}

		public MongoDbWebhookSubscriptionStrore(MongoDbWebhookOptions options) : base(options) {
		}

		protected override IMongoCollection<WebhookSubscriptionDocument> Collection => GetCollection(Options.SubscriptionsCollectionName);

		async Task<IWebhookSubscription> IWebhookSubscriptionStore<IWebhookSubscription>.GetByIdAsync(string id, CancellationToken cancellationToken)
			=> await base.GetByIdAsync(id, cancellationToken);

		public async Task<IList<WebhookSubscriptionDocument>> GetByEventTypeAsync(string eventType, bool activeOnly, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			var filter = Builders<WebhookSubscriptionDocument>.Filter
				.AnyEq(doc => doc.EventTypes, eventType);

			if (activeOnly) {
				var activeFilter = Builders<WebhookSubscriptionDocument>.Filter.Eq(doc => doc.Status, WebhookSubscriptionStatus.Active);
				filter = Builders<WebhookSubscriptionDocument>.Filter.And(filter, activeFilter);
			}

			var result = await Collection.FindAsync(filter, cancellationToken: cancellationToken);
			return await result.ToListAsync(cancellationToken);
		}

		async Task<IList<IWebhookSubscription>> IWebhookSubscriptionStore<IWebhookSubscription>.GetByEventTypeAsync(string eventType, bool activeOnly, CancellationToken cancellationToken) {
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

		IQueryable<IWebhookSubscription> IWebhookSubscriptionQueryableStore<IWebhookSubscription>.AsQueryable() => Collection.AsQueryable();

		async Task<WebhookSubscriptionPage<IWebhookSubscription>> IWebhookSubscriptionPaginatedStore<IWebhookSubscription>.GetPageAsync(WebhookSubscriptionQuery<IWebhookSubscription> query, CancellationToken cancellationToken) {
			var newQuery = new WebhookSubscriptionQuery<WebhookSubscriptionDocument>(query.Page, query.PageSize);

			if (query.Predicate != null) {
				newQuery.Predicate = webhook => query.Predicate.Compile().Invoke(webhook);
			}

			var result = await GetPageAsync(newQuery, cancellationToken);

			return new WebhookSubscriptionPage<IWebhookSubscription>(query, result.TotalCount, result.Subscriptions);
		}

		public IQueryable<WebhookSubscriptionDocument> AsQueryable() => Collection.AsQueryable();

		public async Task<WebhookSubscriptionPage<WebhookSubscriptionDocument>> GetPageAsync(WebhookSubscriptionQuery<WebhookSubscriptionDocument> query, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			var filter = Builders<WebhookSubscriptionDocument>.Filter.Empty;
			if (query.Predicate != null)
				filter = new ExpressionFilterDefinition<WebhookSubscriptionDocument>(query.Predicate);

			var options = new FindOptions<WebhookSubscriptionDocument> {
				Limit = query.PageSize,
				Skip = query.Offset
			};

			var count = await Collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
			var items = await Collection.FindAsync(filter, options, cancellationToken);

			return new WebhookSubscriptionPage<WebhookSubscriptionDocument>(query, (int)count, await items.ToListAsync(cancellationToken));
		}
	}
}
