// Copyright 2022-2023 Deveel
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

using MongoDB.Bson;
using MongoDB.Driver;

using MongoFramework;
using MongoFramework.Linq;

namespace Deveel.Webhooks {
	public class MongoDbWebhookSubscriptionStrore<TSubscription> :
			IWebhookSubscriptionStore<TSubscription>,
			IWebhookSubscriptionQueryableStore<TSubscription>,
			IWebhookSubscriptionPagedStore<TSubscription>
			where TSubscription : MongoWebhookSubscription {

		public MongoDbWebhookSubscriptionStrore(IMongoDbWebhookContext context) {
			Subscriptions = context.Set<TSubscription>();
		}

		protected IMongoDbSet<TSubscription> Subscriptions { get; }

		public IQueryable<TSubscription> AsQueryable() => Subscriptions.AsQueryable();

		public Task<int> CountAllAsync(CancellationToken cancellationToken = default)
			=> Subscriptions.CountAsync(cancellationToken);

		public async Task<string> CreateAsync(TSubscription subscription, CancellationToken cancellationToken = default) {
			Subscriptions.Add(subscription);
			await Subscriptions.Context.SaveChangesAsync(cancellationToken);

			return subscription.Id.ToString();
		}

		public async Task<bool> DeleteAsync(TSubscription subscription, CancellationToken cancellationToken = default) {
			Subscriptions.Remove(subscription);
			await Subscriptions.Context.SaveChangesAsync(cancellationToken);

			return true;
		}

		public async Task<TSubscription> FindByIdAsync(string id, CancellationToken cancellationToken = default)
			=> await Subscriptions.FindAsync(ObjectId.Parse(id));

		public async Task<IList<TSubscription>> GetByEventTypeAsync(string eventType, bool activeOnly, CancellationToken cancellationToken) {
			cancellationToken.ThrowIfCancellationRequested();

			var filter = Builders<TSubscription>.Filter
				.AnyEq(doc => doc.EventTypes, eventType);

			if (activeOnly) {
				var activeFilter = Builders<TSubscription>.Filter.Eq(doc => doc.Status, WebhookSubscriptionStatus.Active);
				filter = Builders<TSubscription>.Filter.And(filter, activeFilter);
			}

			var query = Subscriptions.Where(s => s.EventTypes.Any(y => y == eventType));
			if (activeOnly)
				query = query.Where(s => s.Status == WebhookSubscriptionStatus.Active);

			return await query.ToListAsync(cancellationToken);
		}

		public async Task<PagedResult<TSubscription>> GetPageAsync(PagedQuery<TSubscription> query, CancellationToken cancellationToken) {
			var querySet = Subscriptions.AsQueryable();

			if (query.Predicate != null)
				querySet = querySet.Where(query.Predicate);

			var total = await querySet.CountAsync(cancellationToken);

			querySet = querySet.Skip(query.Offset).Take(query.PageSize);

			var items = await querySet.ToListAsync(cancellationToken);
			return new PagedResult<TSubscription>(query, total, items);
		}

		public Task SetStateAsync(TSubscription subscription, WebhookSubscriptionStatus status, CancellationToken cancellationToken) {
			cancellationToken.ThrowIfCancellationRequested();

			subscription.Status = status;
			subscription.LastStatusTime = DateTimeOffset.UtcNow;

			return Task.CompletedTask;
		}

		public async Task<bool> UpdateAsync(TSubscription subscription, CancellationToken cancellationToken = default) {
			Subscriptions.Update(subscription);

			await Subscriptions.Context.SaveChangesAsync(cancellationToken);

			return true;
		}
	}
}
