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

using Deveel.Data;

using Microsoft.Extensions.Logging;

using MongoDB.Driver;

using MongoFramework;
using MongoFramework.Linq;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides an implementation of the <see cref="IWebhookSubscriptionRepository{TSubscription}"/>
	/// that is backed by a MongoDB database.
	/// </summary>
	/// <typeparam name="TSubscription">
	/// The type of the webhook subscription, that is
	/// derived from <see cref="MongoWebhookSubscription"/>.
	/// </typeparam>
	public class MongoDbWebhookSubscriptionRepository<TSubscription> : 
		MongoRepository<TSubscription>,
		IWebhookSubscriptionRepository<TSubscription>
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
		public MongoDbWebhookSubscriptionRepository(IMongoDbWebhookContext context, ILogger<MongoDbWebhookSubscriptionRepository<TSubscription>>? logger) 
			: base(context, logger) {
		}

		/// <summary>
		/// Gets a set that is used to access the webhook subscriptions
		/// stored in the database.
		/// </summary>
		protected IMongoDbSet<TSubscription> Subscriptions => base.DbSet;

		/// <inheritdoc/>
		public IQueryable<TSubscription> AsQueryable() => Subscriptions.AsQueryable();

		///// <inheritdoc/>
		//public Task<int> CountAllAsync(CancellationToken cancellationToken = default) {
		//	try {
		//		return Subscriptions.CountAsync(cancellationToken);
		//	} catch (Exception ex) {
		//		throw new WebhookMongoException("Could not count the subscriptions", ex);
		//	}
		//}

		/// <inheritdoc/>
		public Task SetDestinationUrlAsync(TSubscription subscription, string url, CancellationToken cancellationToken = default) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			subscription.DestinationUrl = url;

			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		public async Task<IList<TSubscription>> GetByEventTypeAsync(string eventType, bool? activeOnly, CancellationToken cancellationToken) {
			try {
				var query = Subscriptions.Where(s => s.EventTypes.Any(y => y == eventType));
				if (activeOnly ?? false)
					query = query.Where(s => s.Status == WebhookSubscriptionStatus.Active);

				return await query.ToListAsync(cancellationToken);
			} catch (Exception ex) {
				throw new WebhookMongoException("Unable to look for subscriptions to events", ex);
			}
		}

		/// <inheritdoc/>
		public Task SetStatusAsync(TSubscription subscription, WebhookSubscriptionStatus status, CancellationToken cancellationToken) {
			cancellationToken.ThrowIfCancellationRequested();

			subscription.Status = status;
			subscription.LastStatusTime = DateTimeOffset.UtcNow;

			return Task.CompletedTask;
		}
	}
}
