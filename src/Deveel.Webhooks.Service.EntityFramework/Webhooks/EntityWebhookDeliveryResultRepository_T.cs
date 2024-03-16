// Copyright 2022-2024 Antonello Provenzano
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
	/// An implementation of <see cref="IWebhookDeliveryResultRepository{TResult,TKey}"/> that
	/// uses an Entity Framework Core <see cref="DbContext"/> to store the
	/// delivery results of a webhook.
	/// </summary>
	/// <typeparam name="TResult">
	/// The type of delivery result to be stored in the database.
	/// </typeparam>
	public class EntityWebhookDeliveryResultRepository<TResult> : EntityRepository<TResult, int>,
        IWebhookDeliveryResultRepository<TResult, int>
        where TResult : DbWebhookDeliveryResult {

        /// <summary>
        /// Constructs the store with the given <see cref="WebhookDbContext"/>.
        /// </summary>
        /// <param name="context">
		/// The database context to be used to store the delivery results.
		/// </param>
		/// <param name="logger">
		/// An optional logger to be used to log information about the store.
		/// </param>
        public EntityWebhookDeliveryResultRepository(WebhookDbContext context, ILogger<EntityWebhookDeliveryResultRepository<TResult>>? logger = null) 
			: base(context, logger) {
        }

        /// <inheritdoc/>
        public async Task<TResult?> FindByWebhookIdAsync(string webhookId, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            try {
                return await this.FindFirstAsync<TResult, int>(x => x.Webhook.WebhookId == webhookId, cancellationToken);
            } catch (Exception ex) {
                throw new WebhookEntityException("Unable to query the database for results", ex);
            }
        }
    }
}
