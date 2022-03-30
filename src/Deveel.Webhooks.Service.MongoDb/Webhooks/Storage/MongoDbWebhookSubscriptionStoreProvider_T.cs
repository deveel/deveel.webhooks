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

using Microsoft.Extensions.Options;

namespace Deveel.Webhooks.Storage {
	public class MongoDbWebhookSubscriptionStoreProvider<TSubscription> : MongoDbStoreProviderBase<TSubscription, IWebhookSubscription>,
													IWebhookSubscriptionStoreProvider
			where TSubscription : MongoDbWebhookSubscription {
		public MongoDbWebhookSubscriptionStoreProvider(IOptions<MongoDbOptions> baseOptions)
			: base(baseOptions) {
		}

		public MongoDbWebhookSubscriptionStoreProvider(MongoDbOptions baseOptions)
			: base(baseOptions) {
		}

		protected override MongoDbStoreBase<TSubscription> CreateStore(MongoDbOptions options)
			=> new MongoDbWebhookSubscriptionStrore<TSubscription>(options);

		public MongoDbWebhookSubscriptionStrore<TSubscription> GetStore(string tenantId) 
			=> (MongoDbWebhookSubscriptionStrore<TSubscription>)CreateStore(tenantId);

		IWebhookSubscriptionStore<IWebhookSubscription> IWebhookSubscriptionStoreProvider<IWebhookSubscription>.GetTenantStore(string tenantId)
			=> GetStore(tenantId);

		IWebhookStore<IWebhookSubscription> IWebhookStoreProvider<IWebhookSubscription>.GetTenantStore(string tenantId)
			=> GetStore(tenantId);
	}
}
