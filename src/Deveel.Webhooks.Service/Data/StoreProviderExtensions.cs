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
using System.Threading;
using System.Threading.Tasks;

using Deveel.Webhooks.Storage;
using Deveel.Webhooks;

namespace Deveel.Data {
	public static class StoreProviderExtensions {
		public static Task<string> CreateAsync<TEntity>(this IStoreProvider<TEntity> provider, string tenantId, TEntity entity, CancellationToken cancellationToken = default)
			where TEntity : class
			=> provider.GetTenantStore(tenantId).CreateAsync(entity, cancellationToken);

		public static string Create<TEntity>(this IStoreProvider<TEntity> provider, string tenantId, TEntity entity)
			where TEntity : class
			=> provider.GetTenantStore(tenantId).Create(entity);

		public static Task<TEntity> FindByIdAsync<TEntity>(this IStoreProvider<TEntity> provider, string tenantId, string id, CancellationToken cancellationToken = default)
			where TEntity : class
			=> provider.GetTenantStore(tenantId).FindByIdAsync(id, cancellationToken);

		public static TEntity FindById<TEntity>(this IStoreProvider<TEntity> provider, string tenantId, string id)
			where TEntity : class
			=> provider.GetTenantStore(tenantId).FindById(id);

		public static Task<bool> DeleteAsync<TEntity>(this IStoreProvider<TEntity> provider, string tenantId, TEntity entity, CancellationToken cancellationToken = default)
			where TEntity : class
			=> provider.GetTenantStore(tenantId).DeleteAsync(entity, cancellationToken);

		public static Task<bool> UpdateAsync<TEntity>(this IStoreProvider<TEntity> provider, string tenantId, TEntity entity, CancellationToken cancellationToken = default)
			where TEntity : class
			=> provider.GetTenantStore(tenantId).UpdateAsync(entity, cancellationToken);
	}
}
