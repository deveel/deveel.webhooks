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
			if (context is null) 
				throw new ArgumentNullException(nameof(context));

			Subscriptions = context.Set<TSubscription>();
		}

		protected IMongoDbSet<TSubscription> Subscriptions { get; }

		public IQueryable<TSubscription> AsQueryable() => Subscriptions.AsQueryable();

		public Task<int> CountAllAsync(CancellationToken cancellationToken = default) {
			try {
				return Subscriptions.CountAsync(cancellationToken);
			} catch (Exception ex) {
				throw new WebhookMongoException("Could not count the subscriptions", ex);
			}
		}

		public Task<string?> GetIdAsync(TSubscription subscription, CancellationToken cancellationToken) {
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(subscription.Id.ToEntityId());
		}

		public async Task<string> CreateAsync(TSubscription subscription, CancellationToken cancellationToken) {
			try {
				if (subscription.CreatedAt == null)
					subscription.CreatedAt = DateTimeOffset.UtcNow;

				Subscriptions.Add(subscription);
				await Subscriptions.Context.SaveChangesAsync(cancellationToken);

				return subscription.Id.ToString();
			} catch (Exception ex) {
				throw new WebhookMongoException("Could not create the subscription", ex);
			}
		}

		public async Task<bool> DeleteAsync(TSubscription subscription, CancellationToken cancellationToken) {
			try {
				Subscriptions.Remove(subscription);
				await Subscriptions.Context.SaveChangesAsync(cancellationToken);

				return true;
			} catch (Exception ex) {
				throw new WebhookMongoException("Unable to delete the subscription", ex);
			}
		}

		public async Task<TSubscription?> FindByIdAsync(string id, CancellationToken cancellationToken = default) {
			if (string.IsNullOrWhiteSpace(id)) 
				throw new ArgumentException($"'{nameof(id)}' cannot be null or whitespace.", nameof(id));

			if (!ObjectId.TryParse(id, out var objId))
				throw new ArgumentException($"'{nameof(id)}' is not a valid ObjectId.", nameof(id));

			try {
				return await Subscriptions.FindAsync(objId);
			} catch (Exception ex) {
				throw new WebhookMongoException("An error occurred while looking up for the subscription", ex);
			}
		}

		public async Task<IList<TSubscription>> GetByEventTypeAsync(string eventType, bool activeOnly, CancellationToken cancellationToken) {
			try {
				var query = Subscriptions.Where(s => s.EventTypes.Any(y => y == eventType));
				if (activeOnly)
					query = query.Where(s => s.Status == WebhookSubscriptionStatus.Active);

				return await query.ToListAsync(cancellationToken);
			} catch (Exception ex) {
				throw new WebhookMongoException("Unable to look for subscriptions to events", ex);
			}
		}

		public async Task<PagedResult<TSubscription>> GetPageAsync(PagedQuery<TSubscription> query, CancellationToken cancellationToken) {
			try {
				var querySet = Subscriptions.AsQueryable();

				if (query.Predicate != null)
					querySet = querySet.Where(query.Predicate);

				var total = await querySet.CountAsync(cancellationToken);

				var items = await querySet
					.Skip(query.Offset)
					.Take(query.PageSize)
					.ToListAsync(cancellationToken);

				return new PagedResult<TSubscription>(query, total, items);
			} catch (Exception ex) {
				throw new WebhookMongoException("Unable to query the page of subscriptions", ex);
			}
		}

		public Task SetStatusAsync(TSubscription subscription, WebhookSubscriptionStatus status, CancellationToken cancellationToken) {
			cancellationToken.ThrowIfCancellationRequested();

			subscription.Status = status;
			subscription.LastStatusTime = DateTimeOffset.UtcNow;

			return Task.CompletedTask;
		}

		public async Task<bool> UpdateAsync(TSubscription subscription, CancellationToken cancellationToken = default) {
			try {
				// TODO: find a way to check if the entity was updated

				//var entry = Subscriptions.Context.ChangeTracker.GetEntry(subscription);
				//if (entry == null || entry.State != MongoFramework.Infrastructure.EntityEntryState.Updated)
				//	return false;

				subscription.UpdatedAt = DateTimeOffset.UtcNow;

				Subscriptions.Update(subscription);
				await Subscriptions.Context.SaveChangesAsync(cancellationToken);

				return true;
			} catch (Exception ex) {
				throw new WebhookMongoException("Unable to update the subscription", ex);
			}
		}
	}
}
