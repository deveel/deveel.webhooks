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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

using MongoFramework;
using MongoFramework.Linq;

namespace Deveel.Webhooks {
	public class MongoDbWebhookDeliveryResultStore<TResult> : IWebhookDeliveryResultStore<TResult>
		where TResult : MongoWebhookDeliveryResult {
		public MongoDbWebhookDeliveryResultStore(IMongoDbWebhookContext context) {
			Results = context.Set<TResult>();
		}

		protected IMongoDbSet<TResult> Results { get; }

		public Task<int> CountAllAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

		public async Task<string> CreateAsync(TResult result, CancellationToken cancellationToken) {
			Results.Add(result);
			await Results.Context.SaveChangesAsync(cancellationToken);

			return result.Id.ToString();
		}

		public Task<bool> DeleteAsync(TResult result, CancellationToken cancellationToken) => throw new NotImplementedException();
		public Task<TResult> FindByIdAsync(string id, CancellationToken cancellationToken) => throw new NotImplementedException();

		public async Task<TResult> FindByWebhookIdAsync(string webhookId, CancellationToken cancellationToken) {
			cancellationToken.ThrowIfCancellationRequested();

			return await Results.AsQueryable().FirstOrDefaultAsync(x => x.Webhook.WebhookId == webhookId, cancellationToken);
		}
	}
}
