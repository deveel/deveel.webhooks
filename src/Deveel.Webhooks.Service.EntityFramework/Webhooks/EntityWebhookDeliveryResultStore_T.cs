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
    public class EntityWebhookDeliveryResultStore<TResult> : 
        IWebhookDeliveryResultStore<TResult>,
        IWebhookDeliveryResultQueryableStore<TResult>
        where TResult : WebhookDeliveryResultEntity {
        private readonly WebhookDbContext context;

        public EntityWebhookDeliveryResultStore(WebhookDbContext context) {
            this.context = context;
        }

        /// <summary>
        /// Gets the set of results stored in the database.
        /// </summary>
        protected DbSet<TResult> Results => context.Set<TResult>();

        /// <inheritdoc/>
        public IQueryable<TResult> AsQueryable() => Results.AsQueryable();

        /// <inheritdoc/>
        public async Task<int> CountAllAsync(CancellationToken cancellationToken) {
            try {
                return await Results.CountAsync(cancellationToken);
            } catch (Exception ex) {
                throw new WebhookEntityException("Unable to count the number of results", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<string> CreateAsync(TResult result, CancellationToken cancellationToken) {
            try {
                Results.Add(result);
                await context.SaveChangesAsync(cancellationToken);

                return result.Id.ToString();
            } catch (Exception ex) {
                throw new WebhookEntityException("Unable to add the given result to the database", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(TResult result, CancellationToken cancellationToken) {
            try {
                // TODO: verify that the result was registered in the context
                var entry = Results.Entry(result);
                
                if (entry == null)
                    return false;

                entry.State = EntityState.Deleted;

                var count = await context.SaveChangesAsync(true, cancellationToken);
                return count > 0;
            } catch (Exception ex) {
                throw new WebhookEntityException("Unable to delete the result from the database", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<TResult?> FindByIdAsync(string id, CancellationToken cancellationToken) {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            if (!Int32.TryParse(id, out var resultId))
                throw new ArgumentException($"The given id '{id}' is not a valid integer value");

            try {
                return await Results.FindAsync(resultId);
            } catch (Exception ex) {
                throw new WebhookEntityException("An error occurred while looking up for a result", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<TResult?> FindByWebhookIdAsync(string webhookId, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            try {
                return await Results.AsQueryable().FirstOrDefaultAsync(x => x.Webhook.WebhookId == webhookId, cancellationToken);
            } catch (Exception ex) {
                throw new WebhookEntityException("Unable to query the database for results", ex);
            }
        }
    }
}
