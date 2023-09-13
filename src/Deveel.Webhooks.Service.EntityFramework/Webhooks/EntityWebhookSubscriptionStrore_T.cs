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

using Microsoft.EntityFrameworkCore;

namespace Deveel.Webhooks {
    /// <summary>
    /// An implementation of <see cref="IWebhookSubscriptionStore{TSubscription}"/> that
    /// uses an <see cref="DbContext"/> to store the subscriptions.
    /// </summary>
    /// <typeparam name="TSubscription">
    /// The type of the subscription entity to be stored.
    /// </typeparam>
    /// <seealso cref="IWebhookSubscriptionStore{TSubscription}"/>"/>
    public class EntityWebhookSubscriptionStrore<TSubscription> : 
        IWebhookSubscriptionStore<TSubscription>,
        IWebhookSubscriptionQueryableStore<TSubscription>,
        IWebhookSubscriptionPagedStore<TSubscription>
        where TSubscription : DbWebhookSubscription {
        private readonly WebhookDbContext context;

        /// <summary>
        /// Constructs the store by using the given <see cref="WebhookDbContext"/>.
        /// </summary>
        /// <param name="context">
        /// The database context to be used to store the subscriptions.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the given <paramref name="context"/> is <c>null</c>.
        /// </exception>
        public EntityWebhookSubscriptionStrore(WebhookDbContext context) {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Gets the set of subscriptions stored in the database.
        /// </summary>
        protected DbSet<TSubscription> Subscriptions => context.Set<TSubscription>();

        /// <inheritdoc/>
        public IQueryable<TSubscription> AsQueryable() => Subscriptions.AsQueryable();

        /// <inheritdoc/>
        public Task<string?> GetIdAsync(TSubscription subscription, CancellationToken cancellationToken) {
            return Task.FromResult(subscription.Id);
        }

        /// <inheritdoc/>
        public async Task<int> CountAllAsync(CancellationToken cancellationToken = default) {
            try {
                return await Subscriptions.CountAsync(cancellationToken);
            } catch (Exception ex) {
                throw new WebhookEntityException("Could not count the subscriptions", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<TSubscription?> FindByIdAsync(string id, CancellationToken cancellationToken = default) {
            try {
                return await Subscriptions.FindAsync(new object[] { id }, cancellationToken);
            } catch (Exception ex) {
                throw new WebhookEntityException($"Could not find the subscription with id '{id}'", ex);
            }
        }

        /// <inheritdoc/>
        public async Task CreateAsync(TSubscription subscription, CancellationToken cancellationToken = default) {
            try {
                subscription.CreatedAt = DateTimeOffset.UtcNow;

                await Subscriptions.AddAsync(subscription, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
            } catch (Exception ex) {
                throw new WebhookEntityException("Could not create the subscription", ex);
            }
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(TSubscription subscription, CancellationToken cancellationToken = default) {
            try {
                subscription.UpdatedAt = DateTimeOffset.UtcNow;

                Subscriptions.Update(subscription);
                await context.SaveChangesAsync(cancellationToken);
            } catch (Exception ex) {
                throw new WebhookEntityException("Could not update the subscription", ex);
            }
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(TSubscription subscription, CancellationToken cancellationToken = default) {
            try {
                Subscriptions.Remove(subscription);
                await context.SaveChangesAsync(cancellationToken);
            } catch (Exception ex) {
                throw new WebhookEntityException("Could not delete the subscription", ex);
            }
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

        /// <inheritdoc/>
        public async Task<PagedResult<TSubscription>> GetPageAsync(PagedQuery<TSubscription> query, CancellationToken cancellationToken) {
            var dbSet = Subscriptions.AsQueryable();
            if (query.Predicate != null)
                dbSet = dbSet.Where(query.Predicate);

            var count = await dbSet.CountAsync(cancellationToken);
            var items = await dbSet.Skip(query.Offset)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<TSubscription>(query, count, items);
        }
    }
}
