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

using Microsoft.Extensions.Logging;

using MongoDB.Bson;
using MongoDB.Driver;

using MongoFramework;
using MongoFramework.Linq;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides an implementation of the <see cref="IWebhookDeliveryResultRepository{TResult}"/>
	/// that is backed by a MongoDB database.
	/// </summary>
	/// <typeparam name="TResult">
	/// The type of the result that is stored in the database.
	/// </typeparam>
	public class MongoDbWebhookDeliveryResultRepository<TResult> : MongoRepository<TResult>,
		IWebhookDeliveryResultRepository<TResult>
		where TResult : MongoWebhookDeliveryResult {

		/// <summary>
		/// Constructs the store with the given context.
		/// </summary>
		/// <param name="context">
		/// The context to the MongoDB database.
		/// </param>
		/// <param name="logger">
		/// An object to use for logging operations.
		/// </param>
		public MongoDbWebhookDeliveryResultRepository(IMongoDbWebhookContext context, ILogger<MongoDbWebhookDeliveryResultRepository<TResult>>? logger = null) 
			: base(context, logger) {
		}

		/// <summary>
		/// Gets the set of results stored in the database.
		/// </summary>
		protected IMongoDbSet<TResult> Results => base.DbSet;

		/// <inheritdoc/>
		public async Task<TResult?> FindByWebhookIdAsync(string webhookId, CancellationToken cancellationToken) {
			cancellationToken.ThrowIfCancellationRequested();

			try {
				return await Results.AsQueryable().FirstOrDefaultAsync(x => x.Webhook.WebhookId == webhookId, cancellationToken);
			} catch (Exception ex) {
				throw new WebhookMongoException("Unable to query the database for results", ex);
			}

		}
	}
}