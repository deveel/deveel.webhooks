// Copyright 2022 Deveel
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
using System.Threading.Tasks;
using System.Threading;

using Microsoft.Extensions.Options;
using System.Linq;
using MongoDB.Driver;
using Deveel.Webhooks;

namespace Deveel.Data {
	public abstract class MongoDbStoreBase<TDocument, TFacade> : MongoDbStoreBase<TDocument>
		where TDocument : class, TFacade, IMongoDocument
		where TFacade : class {
		protected MongoDbStoreBase(IOptions<MongoDbOptions> options) : base(options) {
		}

		protected MongoDbStoreBase(MongoDbOptions options) : base(options) {
		}

		public Task<string> CreateAsync(TFacade entity, CancellationToken cancellationToken)
			=> base.CreateAsync((TDocument)entity, cancellationToken);

		public Task<bool> UpdateAsync(TFacade facade, CancellationToken cancellationToken)
			=> base.UpdateAsync((TDocument)facade, cancellationToken);

		public Task<bool> DeleteAsync(TFacade entity, CancellationToken cancellationToken)
			=> base.DeleteAsync((TDocument)entity, cancellationToken);

		//async Task<TFacade> IWebhookStore<TFacade>.FindByIdAsync(string id, CancellationToken cancellationToken)
		//	=> await base.FindByIdAsync(id, cancellationToken);

		public async Task<PagedResult<TFacade>> GetPageAsync(PagedQuery<TFacade> query, CancellationToken cancellationToken) {
			var newQuery = new PagedQuery<TDocument>(query.Page, query.PageSize);
			if (query.Predicate != null)
				newQuery.Predicate = item => query.Predicate.Compile().Invoke(item);

			var result = await base.GetPageAsync(newQuery, cancellationToken);

			var items = result.Items.Cast<TFacade>();
			return new PagedResult<TFacade>(query, result.TotalCount, items);
		}
	}
}
