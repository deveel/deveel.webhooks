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

using Deveel.Webhooks;

using Microsoft.Extensions.Options;

namespace Deveel.Data {
	public abstract class MongoDbStoreProviderBase<TDocument>
		where TDocument : class, IMongoDocument {
		protected MongoDbStoreProviderBase(IOptions<MongoDbOptions> options)
	: this(options.Value) {
		}

		protected MongoDbStoreProviderBase(MongoDbOptions options) {
			Options = options;
		}

		protected MongoDbOptions Options { get; }

		//IWebhookStore<TDocument> IWebhookStoreProvider<TDocument>.GetTenantStore(string tenantId) => CreateStore(tenantId);

		protected MongoDbStoreBase<TDocument> CreateStore(string tenantId) {
			if (string.IsNullOrWhiteSpace(tenantId))
				throw new ArgumentException($"'{nameof(tenantId)}' cannot be null or whitespace.", nameof(tenantId));

			var storeOptions = new MongoDbOptions {
				TenantId = tenantId,
				ConnectionString = Options.ConnectionString,
				DatabaseName = Options.DatabaseName,
				Collections = Options.Collections,
				MultiTenancy = Options.MultiTenancy
			};

			return CreateStore(storeOptions);
		}

		protected abstract MongoDbStoreBase<TDocument> CreateStore(MongoDbOptions options);

	}
}