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
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	abstract class MongoDbWebhookStoreProviderBase<TDocument, TFacade>
		where TDocument : class, TFacade, IMongoDocument
		where TFacade : class {
		protected MongoDbWebhookStoreProviderBase(IOptions<MongoDbWebhookOptions> options)
			: this(options.Value) {
		}

		protected MongoDbWebhookStoreProviderBase(MongoDbWebhookOptions options) {
			Options = options;
		}

		protected MongoDbWebhookOptions Options { get; }

		protected MongoDbWebhookStoreBase<TDocument, TFacade> CreateStore(string tenantId) {
			if (string.IsNullOrWhiteSpace(tenantId))
				throw new ArgumentException($"'{nameof(tenantId)}' cannot be null or whitespace.", nameof(tenantId));

			var storeOptions = new MongoDbWebhookOptions {
				TenantId = tenantId,
				ConnectionString = Options.ConnectionString,
				DatabaseName = Options.DatabaseName,
				SubscriptionsCollectionName = Options.SubscriptionsCollectionName,
				WebhooksCollectionName = Options.WebhooksCollectionName,
			};

			return CreateStore(storeOptions);
		}

		protected abstract MongoDbWebhookStoreBase<TDocument, TFacade> CreateStore(MongoDbWebhookOptions options);
	}
}
