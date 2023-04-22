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

using System;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Data;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

namespace Deveel.Webhooks {
	public class MongoDbWebhookDeliveryResultStore<TResult> : MongoDbStoreBase<TResult>, IWebhookDeliveryResultStore<TResult>
		where TResult : MongoDbWebhookDeliveryResult {
		public MongoDbWebhookDeliveryResultStore(IOptions<MongoDbOptions> options) : base(options) {
		}

		public MongoDbWebhookDeliveryResultStore(MongoDbOptions options) : base(options) {
		}

		protected override IMongoCollection<TResult> Collection => GetCollection(Options.DeliveryResultsCollectionName());

		public async Task<TResult> FindByWebhookIdAsync(string webhookId, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			var filter = Builders<TResult>.Filter.Eq(x => x.Webhook.WebhookId, webhookId);
			filter = NormalizeFilter(filter);

			var result = await Collection.FindAsync(filter, cancellationToken: cancellationToken);

			return await result.FirstOrDefaultAsync(cancellationToken);
		}
	}
}
