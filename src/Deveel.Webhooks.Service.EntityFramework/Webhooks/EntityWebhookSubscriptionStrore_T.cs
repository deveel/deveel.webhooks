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

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Deveel.Webhooks {
    /// <summary>
    /// An implementation of <see cref="IWebhookSubscriptionRepository{TSubscription}"/> that
    /// uses an <see cref="DbContext"/> to store the subscriptions.
    /// </summary>
    /// <typeparam name="TSubscription">
    /// The type of the subscription entity to be stored.
    /// </typeparam>
    /// <seealso cref="IWebhookSubscriptionRepository{TSubscription}"/>"/>
    public class EntityWebhookSubscriptionStrore<TSubscription> : 
		EntityRepository<TSubscription>,
        IWebhookSubscriptionRepository<TSubscription>
        where TSubscription : DbWebhookSubscription {

        /// <summary>
        /// Constructs the store by using the given <see cref="WebhookDbContext"/>.
        /// </summary>
        /// <param name="context">
        /// The database context to be used to store the subscriptions.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the given <paramref name="context"/> is <c>null</c>.
        /// </exception>
        public EntityWebhookSubscriptionStrore(WebhookDbContext context, ILogger<EntityWebhookSubscriptionStrore<TSubscription>>? logger = null) 
			: base(context, logger) {
        }

		/// <summary>
		/// Gets the set of subscriptions stored in the database.
		/// </summary>
		protected DbSet<TSubscription> Subscriptions => base.Entities;

		/// <inheritdoc/>
		public Task SetDestinationUrlAsync(TSubscription subscription, string destinationUrl, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			subscription.DestinationUrl = destinationUrl;

			return Task.CompletedTask;
		}

        /// <inheritdoc/>
        public async Task<IList<TSubscription>> GetByEventTypeAsync(string eventType, bool? activeOnly, CancellationToken cancellationToken = default) {
            try {
                IQueryable<TSubscription> query = Subscriptions.Where(x => x.Events.Any(y => y.EventType == eventType));
                if (activeOnly ?? false)
                    query = query.Where(x => x.Status == WebhookSubscriptionStatus.Active);

                return await query.ToListAsync(cancellationToken);
            } catch (Exception ex) {
                throw new WebhookEntityException($"Could not get the subscriptions for event type '{eventType}'", ex);
            }
        }

        /// <inheritdoc/>
        public Task SetStatusAsync(TSubscription subscription, WebhookSubscriptionStatus status, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            subscription.Status = status;
            return Task.CompletedTask;
        }
    }
}
