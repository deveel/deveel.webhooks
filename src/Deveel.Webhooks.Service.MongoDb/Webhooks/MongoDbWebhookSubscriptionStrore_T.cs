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
using MongoFramework.Infrastructure;
using MongoFramework.Linq;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides an implementation of the <see cref="IWebhookSubscriptionStore{TSubscription}"/>
	/// that is backed by a MongoDB database.
	/// </summary>
	/// <typeparam name="TSubscription">
	/// The type of the webhook subscription, that is
	/// derived from <see cref="MongoWebhookSubscription"/>.
	/// </typeparam>
	public class MongoDbWebhookSubscriptionStrore<TSubscription> :
			IWebhookSubscriptionStore<TSubscription>,
			IWebhookSubscriptionQueryableStore<TSubscription>,
			IWebhookSubscriptionPagedStore<TSubscription>
			where TSubscription : MongoWebhookSubscription {

		/// <summary>
		/// Constructs the store with the given context.
		/// </summary>
		/// <param name="context">
		/// The context that is used to access the MongoDB database.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the given <paramref name="context"/> is <c>null</c>.
		/// </exception>
		public MongoDbWebhookSubscriptionStrore(IMongoDbWebhookContext context) {
			if (context is null) 
				throw new ArgumentNullException(nameof(context));

			Subscriptions = context.Set<TSubscription>();
		}

		/// <summary>
		/// Gets a set that is used to access the webhook subscriptions
		/// stored in the database.
		/// </summary>
		protected IMongoDbSet<TSubscription> Subscriptions { get; }

		/// <inheritdoc/>
		public IQueryable<TSubscription> AsQueryable() => Subscriptions.AsQueryable();

		/// <inheritdoc/>
		public Task<int> CountAllAsync(CancellationToken cancellationToken = default) {
			try {
				return Subscriptions.CountAsync(cancellationToken);
			} catch (Exception ex) {
				throw new WebhookMongoException("Could not count the subscriptions", ex);
			}
		}

		/// <inheritdoc/>
		public Task<string?> GetIdAsync(TSubscription subscription, CancellationToken cancellationToken) {
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(subscription.Id.ToEntityId());
		}

		/// <inheritdoc/>
		public async Task CreateAsync(TSubscription subscription, CancellationToken cancellationToken) {
			try {
				if (subscription.CreatedAt == null)
					subscription.CreatedAt = DateTimeOffset.UtcNow;

				Subscriptions.Add(subscription);
				await Subscriptions.Context.SaveChangesAsync(cancellationToken);
			} catch (Exception ex) {
				throw new WebhookMongoException("Could not create the subscription", ex);
			}
		}

		/// <inheritdoc/>
		public async Task DeleteAsync(TSubscription subscription, CancellationToken cancellationToken) {
			try {
				// TODO: Check if the subscription already exists
				var entry = Subscriptions.Context.ChangeTracker.GetEntry(subscription);
				if (entry != null && entry.State == EntityEntryState.Deleted)
                    throw new WebhookMongoException("The subscription was already deleted");

				Subscriptions.Remove(subscription);
				await Subscriptions.Context.SaveChangesAsync(cancellationToken);
			} catch (Exception ex) {
				throw new WebhookMongoException("Unable to delete the subscription", ex);
			}
		}

        /// <inheritdoc/>
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

		/// <inheritdoc/>
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

		/// <inheritdoc/>
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

		/// <inheritdoc/>
		public Task SetStatusAsync(TSubscription subscription, WebhookSubscriptionStatus status, CancellationToken cancellationToken) {
			cancellationToken.ThrowIfCancellationRequested();

			subscription.Status = status;
			subscription.LastStatusTime = DateTimeOffset.UtcNow;

			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		public async Task UpdateAsync(TSubscription subscription, CancellationToken cancellationToken = default) {
			try {
				// TODO: find a way to check if the entity was updated

				//var entry = Subscriptions.Context.ChangeTracker.GetEntry(subscription);
				//if (entry == null || entry.State != MongoFramework.Infrastructure.EntityEntryState.Updated)
				//	return false;

				subscription.UpdatedAt = DateTimeOffset.UtcNow;

				Subscriptions.Update(subscription);
				await Subscriptions.Context.SaveChangesAsync(cancellationToken);
			} catch (Exception ex) {
				throw new WebhookMongoException("Unable to update the subscription", ex);
			}
		}
	}
}
