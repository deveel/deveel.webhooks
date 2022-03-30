﻿// Copyright 2022 Deveel
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

using Deveel.Data;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

namespace Deveel.Webhooks {
	public class MongoDbWebhookSubscriptionStrore<TSubscription> : MongoDbStoreBase<TSubscription, IWebhookSubscription>,
											 IWebhookSubscriptionStore,
											 IWebhookSubscriptionQueryableStore<IWebhookSubscription>
			where TSubscription : MongoDbWebhookSubscription {
		public MongoDbWebhookSubscriptionStrore(IOptions<MongoDbOptions> options) : base(options) {
		}

		public MongoDbWebhookSubscriptionStrore(MongoDbOptions options) : base(options) {
		}

		protected override IMongoCollection<TSubscription> Collection => GetCollection(Options.SubscriptionsCollectionName());



		public async Task<IList<TSubscription>> GetByEventTypeAsync(string eventType, bool activeOnly, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			var filter = Builders<TSubscription>.Filter
				.AnyEq(doc => doc.EventTypes, eventType);

			if (activeOnly) {
				var activeFilter = Builders<TSubscription>.Filter.Eq(doc => doc.Status, WebhookSubscriptionStatus.Active);
				filter = Builders<TSubscription>.Filter.And(filter, activeFilter);
			}

			filter = NormalizeFilter(filter);

			var result = await Collection.FindAsync(filter, cancellationToken: cancellationToken);
			return await result.ToListAsync(cancellationToken);
		}

		async Task<IList<IWebhookSubscription>> IWebhookSubscriptionStore<IWebhookSubscription>.GetByEventTypeAsync(string eventType, bool activeOnly, CancellationToken cancellationToken) {
			var result = await GetByEventTypeAsync(eventType, activeOnly, cancellationToken);
			return result.Cast<IWebhookSubscription>().ToList();
		}

		public Task SetStateAsync(TSubscription subscription, WebhookSubscriptionStateInfo stateInfo, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			subscription.Status = stateInfo.Status;
			subscription.LastStatusTime = stateInfo.TimeStamp;

			return Task.CompletedTask;
		}

		Task IWebhookSubscriptionStore<IWebhookSubscription>.SetStateAsync(IWebhookSubscription subscription, WebhookSubscriptionStateInfo stateInfo, CancellationToken cancellationToken)
			=> SetStateAsync((TSubscription)subscription, stateInfo, cancellationToken);

		IQueryable<IWebhookSubscription> IWebhookSubscriptionQueryableStore<IWebhookSubscription>.AsQueryable() => Collection.AsQueryable();

		async Task<IWebhookSubscription> IWebhookSubscriptionStore<IWebhookSubscription>.FindByIdAsync(string id, CancellationToken cancellationToken)
			=> await FindByIdAsync(id, cancellationToken);

		//async Task<PagedResult<IWebhookSubscription>> IStore<IWebhookSubscription>.GetPageAsync(PagedQuery<IWebhookSubscription> query, CancellationToken cancellationToken) {
		//	var newQuery = new PagedQuery<MongoDbWebhookSubscription>(query.Page, query.PageSize);
		//	if (query.Predicate != null)
		//		newQuery.Predicate = item => query.Predicate.Compile().Invoke(item);

		//	var result = await GetPageAsync(newQuery, cancellationToken);

		//	return new PagedResult<IWebhookSubscription>(query, result.TotalCount, result.Items?.Cast<IWebhookSubscription>());
		//}

		//async Task<IWebhookSubscription> IStore<IWebhookSubscription>.FindByIdAsync(string id, CancellationToken cancellationToken) 
		//	=> await base.FindByIdAsync(id, cancellationToken);
	}
}
