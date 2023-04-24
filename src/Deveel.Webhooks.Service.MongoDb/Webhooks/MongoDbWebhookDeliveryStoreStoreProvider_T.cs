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

using Finbuckle.MultiTenant;

namespace Deveel.Webhooks {
    public class MongoDbWebhookDeliveryResultStoreProvider<TTenantInfo, TResult> : IWebhookDeliveryResultStoreProvider<TResult>
		where TTenantInfo : class, ITenantInfo, new()
		where TResult : MongoWebhookDeliveryResult {
		private readonly IMultiTenantStore<TTenantInfo> tenantStore;

		public MongoDbWebhookDeliveryResultStoreProvider(IMultiTenantStore<TTenantInfo> tenantStore) {
			this.tenantStore = tenantStore;
		}

		public IWebhookDeliveryResultStore<TResult> GetTenantStore(string tenantId) {
			var tenantInfo = tenantStore.TryGetByIdentifierAsync(tenantId).GetAwaiter().GetResult();
			if (tenantInfo == null)
				throw new WebhookException($"Tenant '{tenantId}' not found");

			var context = new MultiTenantContext<TTenantInfo> {
				TenantInfo = tenantInfo
			};

			return new MongoDbWebhookDeliveryResultStore<TResult>(new MongoDbWebhookTenantContext<TTenantInfo>(context));
		}
	}
}
