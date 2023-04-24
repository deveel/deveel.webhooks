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
    public class MongoDbWebhookSubscriptionStoreProvider<TTenantInfo, TSubscription> : IWebhookSubscriptionStoreProvider<TSubscription>, IDisposable
		where TTenantInfo : class, ITenantInfo, new()
		where TSubscription : MongoWebhookSubscription {
		private readonly IMultiTenantStore<TTenantInfo> tenantStore;
		private Dictionary<string, IWebhookSubscriptionStore<TSubscription>>? stores;
		private bool disposedValue;

		public MongoDbWebhookSubscriptionStoreProvider(IMultiTenantStore<TTenantInfo> tenantStore) {
			this.tenantStore = tenantStore;
		}

		~MongoDbWebhookSubscriptionStoreProvider() {
			Dispose(disposing: false);
		}

		public IWebhookSubscriptionStore<TSubscription> GetTenantStore(string tenantId) {
			if (!(stores?.TryGetValue(tenantId, out var store) ?? false)) {
				var tenantInfo = tenantStore.TryGetByIdentifierAsync(tenantId).GetAwaiter().GetResult();
				if (tenantInfo == null)
					throw new ArgumentException($"Tenant '{tenantId}' not found");

				var context = new MultiTenantContext<TTenantInfo> {
					TenantInfo = tenantInfo
				};

				store = new MongoDbWebhookSubscriptionStrore<TSubscription>(new MongoDbWebhookTenantContext<TTenantInfo>(context));

				if (stores == null)
					stores = new Dictionary<string, IWebhookSubscriptionStore<TSubscription>>();

				stores[tenantId] = store;
			}

			return store;
		}

		protected virtual void Dispose(bool disposing) {
			if (!disposedValue) {
				if (disposing) {
					DisposeStores();
				}

				stores = null;
				disposedValue = true;
			}
		}

		private void DisposeStores() {
			if (stores != null) {
				foreach (var store in stores.Values) {
					(store as IDisposable)?.Dispose();
				}

				stores.Clear();
			}
		}

		public void Dispose() {
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
