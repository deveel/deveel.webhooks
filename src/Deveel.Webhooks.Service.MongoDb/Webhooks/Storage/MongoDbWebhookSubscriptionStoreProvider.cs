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

using Deveel.Data;
using Deveel.Webhooks.Storage;

using Microsoft.Extensions.Options;

namespace Deveel.Webhooks.Storage {
	class MongoDbWebhookSubscriptionStoreProvider : MongoDbStoreProviderBase<WebhookSubscriptionDocument, IWebhookSubscription>,
													IWebhookSubscriptionStoreProvider,
													IWebhookSubscriptionStoreProvider<WebhookSubscriptionDocument> {
		public MongoDbWebhookSubscriptionStoreProvider(IOptions<MongoDbOptions> baseOptions)
			: base(baseOptions) {
		}

		public MongoDbWebhookSubscriptionStoreProvider(MongoDbOptions baseOptions)
			: base(baseOptions) {
		}

		protected override MongoDbStoreBase<WebhookSubscriptionDocument, IWebhookSubscription> CreateStore(MongoDbOptions options)
			=> new MongoDbWebhookSubscriptionStrore(options);

		public MongoDbWebhookSubscriptionStrore GetStore(string tenantId) => (MongoDbWebhookSubscriptionStrore)CreateStore(tenantId);

		IWebhookSubscriptionStore<IWebhookSubscription> IWebhookSubscriptionStoreProvider<IWebhookSubscription>.GetTenantStore(string tenantId)
			=> GetStore(tenantId);

		IWebhookSubscriptionStore<WebhookSubscriptionDocument> IWebhookSubscriptionStoreProvider<WebhookSubscriptionDocument>.GetTenantStore(string tenantId)
			=> GetStore(tenantId);
	}
}
