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

namespace Deveel.Webhooks {
	/// <summary>
	/// An implementation of <see cref="IWebhookDeliveryResultRepository{TResult}"/> that
	/// uses an Entity Framework Core <see cref="DbContext"/> to store the
	/// delivery results of a webhook.
	/// </summary>
	/// <typeparam name="TResult">
	/// The type of delivery result to be stored in the database.
	/// </typeparam>
	public class EntityWebhookDeliveryResultStore<TResult> : EntityRepository<TResult>,
        IWebhookDeliveryResultRepository<TResult>
        where TResult : DbWebhookDeliveryResult {
        private readonly WebhookDbContext context;

        /// <summary>
        /// Constructs the store with the given <see cref="WebhookDbContext"/>.
        /// </summary>
        /// <param name="context"></param>
        public EntityWebhookDeliveryResultStore(WebhookDbContext context) : base(context) {
            this.context = context;
        }

        /// <summary>
        /// Gets the set of results stored in the database.
        /// </summary>
        protected DbSet<TResult> Results => context.Set<TResult>();

		protected override DbSet<TResult> Entities => Results;

        /// <inheritdoc/>
        public async Task<TResult?> FindByWebhookIdAsync(string webhookId, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            try {
                return await this.FindFirstAsync<TResult>(x => x.Webhook.WebhookId == webhookId, cancellationToken);
            } catch (Exception ex) {
                throw new WebhookEntityException("Unable to query the database for results", ex);
            }
        }
    }
}
