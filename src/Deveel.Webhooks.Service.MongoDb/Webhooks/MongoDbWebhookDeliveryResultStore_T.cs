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
    /// <summary>
    /// Provides an implementation of the <see cref="IWebhookDeliveryResultStore{TResult}"/>
    /// that is backed by a MongoDB database.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of the result that is stored in the database.
    /// </typeparam>
    public class MongoDbWebhookDeliveryResultStore<TResult> : 
	    IWebhookDeliveryResultStore<TResult>, 
	    IWebhookDeliveryResultQueryableStore<TResult>
		where TResult : MongoWebhookDeliveryResult {
		/// <summary>
		/// Constructs the store with the given context.
		/// </summary>
		/// <param name="context">
		/// The context to the MongoDB database.
		/// </param>
		public MongoDbWebhookDeliveryResultStore(IMongoDbWebhookContext context)
		{
			if (context == null) 
				throw new ArgumentNullException(nameof(context));
			
			Results = context.Set<TResult>();
		}

		/// <summary>
		/// Gets the set of results stored in the database.
		/// </summary>
		protected IMongoDbSet<TResult> Results { get; }

		/// <inheritdoc/>
		public IQueryable<TResult> AsQueryable() => Results.AsQueryable();

		/// <inheritdoc/>
		public async Task<int> CountAllAsync(CancellationToken cancellationToken) {
			try {
				return await Results.CountAsync(cancellationToken);
			} catch (Exception ex) {
				throw new WebhookMongoException("Unable to count the number of results", ex);
			}
		}

		/// <inheritdoc/>
		public async Task<string> CreateAsync(TResult result, CancellationToken cancellationToken) {
			try {
                Results.Add(result);
                await Results.Context.SaveChangesAsync(cancellationToken);

                return result.Id.ToString();
            } catch (Exception ex) {
				throw new WebhookMongoException("Unable to add the given result to the database", ex);
			}
		}

		/// <inheritdoc/>
		public async Task<bool> DeleteAsync(TResult result, CancellationToken cancellationToken) {
			try {
				// TODO: verify that the result was registered in the context
				Results.Remove(result);
				await Results.Context.SaveChangesAsync(cancellationToken);

				return true;
			} catch (Exception ex) {
				throw new WebhookMongoException("Unable to delete the result from the database", ex);
			}
		}
		
		/// <inheritdoc/>
		public async Task<TResult?> FindByIdAsync(string id, CancellationToken cancellationToken) {
			if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

			if (!ObjectId.TryParse(id, out var objId))
				throw new ArgumentException("The given id is not a valid ObjectId", nameof(id));

			try {
				return await Results.FindAsync(objId);
			} catch (Exception ex) {
				throw new WebhookMongoException("An error occurred while looking up for a result", ex);
			}
		}

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
