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

using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
    /// <summary>
    /// Provides an implementation of the <see cref="IWebhookDeliveryResultStoreProvider{TResult}"/>
    /// that is resolving instances of <see cref="MongoDbWebhookDeliveryResultStore{TResult}"/> based
    /// on the tenant identifier.
    /// </summary>
    /// <typeparam name="TTenantInfo">
    /// The type of tenant that owns the connection to the MongoDB database
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the result that is stored in the database.
    /// </typeparam>
    public class MongoDbWebhookDeliveryResultStoreProvider<TTenantInfo, TResult> : IWebhookDeliveryResultStoreProvider<TResult>, IDisposable
		where TTenantInfo : class, ITenantInfo, new()
		where TResult : MongoWebhookDeliveryResult {
		private Dictionary<string, MongoDbWebhookDeliveryResultStore<TResult>>? cache;
		private readonly IMultiTenantStore<TTenantInfo> tenantStore;
        private readonly IOptions<MongoDbWebhookOptions> options;
        private bool disposedValue;

        /// <summary>
        /// Constructs the provider with the given tenant store.
        /// </summary>
        /// <param name="options">
        /// The configured options for the connection to the MongoDB database.
        /// </param>
        /// <param name="tenantStore">
        /// The store that is used to resolve the tenant information.
        /// </param>
        public MongoDbWebhookDeliveryResultStoreProvider(IOptions<MongoDbWebhookOptions> options, IMultiTenantStore<TTenantInfo> tenantStore) {
            this.options = options;
            this.tenantStore = tenantStore;
		}

        /// <summary>
        /// Destroys the instance of the provider.
        /// </summary>
        ~MongoDbWebhookDeliveryResultStoreProvider() {
            Dispose(disposing: false);
        }

        /// <summary>
        /// Throws an exception if the instance of the provider is disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Thrown when the instance of the provider is disposed.
        /// </exception>
        protected void ThrowIfDisposed() {
            if (disposedValue)
                throw new ObjectDisposedException(GetType().FullName);
        }

        /// <inheritdoc/>
        public IWebhookDeliveryResultStore<TResult> GetTenantStore(string tenantId) {
            ThrowIfDisposed();

            if (cache == null)
                cache = new Dictionary<string, MongoDbWebhookDeliveryResultStore<TResult>>();

			if (!cache.TryGetValue(tenantId, out var store)) {
				var tenantInfo = tenantStore.TryGetByIdentifierAsync(tenantId).GetAwaiter().GetResult();
				if (tenantInfo == null)
					throw new WebhookException($"Tenant '{tenantId}' not found");

				var context = new MultiTenantContext<TTenantInfo> {
					TenantInfo = tenantInfo
				};

				cache[tenantId] = store = new MongoDbWebhookDeliveryResultStore<TResult>(new MongoDbWebhookTenantContext<TTenantInfo>(options, context));
			}

			return store;
		}

        /// <summary>
        /// Disposes the instance of the provider.
        /// </summary>
        /// <param name="disposing">
        /// A flag indicating if the provider is disposing.
        /// </param>
        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    DisposeStores();
                }

                cache = null;
                disposedValue = true;
            }
        }

        private void DisposeStores() {
            if (cache != null) {
                foreach (var store in cache.Values) {
                    (store as IDisposable)?.Dispose();
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
