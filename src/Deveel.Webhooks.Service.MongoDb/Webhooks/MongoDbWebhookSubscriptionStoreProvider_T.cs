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
	/// A service that implements the <see cref="IWebhookSubscriptionStoreProvider{TSubscription}"/>
	/// contract to resolve a store for a given tenant.
	/// </summary>
	/// <typeparam name="TTenantInfo">
	/// The type of the tenant information used to identify the MongoDB database
	/// </typeparam>
	/// <typeparam name="TSubscription">
	/// The type of the subscription to store.
	/// </typeparam>
    public class MongoDbWebhookSubscriptionStoreProvider<TTenantInfo, TSubscription> : IWebhookSubscriptionStoreProvider<TSubscription>, IDisposable
		where TTenantInfo : class, ITenantInfo, new()
		where TSubscription : MongoWebhookSubscription {
		private readonly IOptions<MongoDbWebhookOptions> options;
		private readonly IMultiTenantStore<TTenantInfo> tenantStore;
		private Dictionary<string, IWebhookSubscriptionStore<TSubscription>>? cache;
		private bool disposedValue;

		/// <summary>
		/// Constructs a new instance of the store provider.
		/// </summary>
		/// <param name="options">
		/// The set of options that are used to configure the connection to the store.
		/// </param>
		/// <param name="tenantStore">
		/// The store that provides access to the tenants information.
		/// </param>
		public MongoDbWebhookSubscriptionStoreProvider(IOptions<MongoDbWebhookOptions> options, IMultiTenantStore<TTenantInfo> tenantStore) {
            this.options = options;
            this.tenantStore = tenantStore;
		}

		/// <summary>
		/// Destroyes the provider.
		/// </summary>
		~MongoDbWebhookSubscriptionStoreProvider() {
			Dispose(disposing: false);
		}

		/// <summary>
		/// Throws an exception if the provider instance was disposed.
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		/// Thrown when the provider instance was disposed.
		/// </exception>
		protected void ThrowIfDisposed() {
			if (disposedValue)
				throw new ObjectDisposedException(GetType().Name);
		}

		/// <inheritdoc/>
		public IWebhookSubscriptionStore<TSubscription> GetTenantStore(string tenantId) {
			if (!(cache?.TryGetValue(tenantId, out var store) ?? false)) {
				var tenantInfo = tenantStore.TryGetByIdentifierAsync(tenantId).GetAwaiter().GetResult();
				if (tenantInfo == null)
					throw new ArgumentException($"Tenant '{tenantId}' not found");

				var context = new MultiTenantContext<TTenantInfo> {
					TenantInfo = tenantInfo
				};

				store = new MongoDbWebhookSubscriptionStrore<TSubscription>(new MongoDbWebhookTenantContext<TTenantInfo>(options, context));

				if (cache == null)
					cache = new Dictionary<string, IWebhookSubscriptionStore<TSubscription>>();

				cache[tenantId] = store;
			}

			return store;
		}

		/// <summary>
		/// Disposes the provider instance.
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

				cache.Clear();
			}
		}

		/// <inheritdoc/>
		public void Dispose() {
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
