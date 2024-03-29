// Copyright 2022-2024 Antonello Provenzano
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

using Deveel.Data;

using Microsoft.Extensions.Logging;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides an implementation of the <see cref="IWebhookDeliveryResultRepositoryProvider{TResult,TKey}"/>
	/// that is resolving instances of <see cref="MongoDbWebhookDeliveryResultRepository{TResult,TKey}"/> based
	/// on the tenant identifier.
	/// </summary>
	/// <typeparam name="TResult">
	/// The type of the result that is stored in the database.
	/// </typeparam>
	/// <typeparam name="TKey">
	/// The type of the key that is used to identify the result in the database.
	/// </typeparam>
	public class MongoDbWebhookDeliveryResultRepositoryProvider<TResult, TKey> : MongoRepositoryProvider<MongoDbWebhookTenantContext, TResult, TKey>, 
		IWebhookDeliveryResultRepositoryProvider<TResult, TKey>
		where TResult : MongoWebhookDeliveryResult
		where TKey : notnull {
        /// <summary>
        /// Constructs the provider with the given tenant store.
        /// </summary>
		/// <param name="tenantResolver">
		/// A service that is used to resolve the tenant identifier for the
		/// current context.
		/// </param>
		/// <param name="loggerFactory">
		/// An object that is used to create the logger for the repositories
		/// created by this provider.
		/// </param>
        public MongoDbWebhookDeliveryResultRepositoryProvider(
			IRepositoryTenantResolver tenantResolver, 
			ILoggerFactory? loggerFactory = null)
			: base(tenantResolver, loggerFactory) {
		}

        /// <inheritdoc/>
        public new async Task<IWebhookDeliveryResultRepository<TResult, TKey>> GetRepositoryAsync(string tenantId, CancellationToken cancellationToken = default) {
			return (IWebhookDeliveryResultRepository<TResult, TKey>) await base.GetRepositoryAsync(tenantId, cancellationToken);
		}

		/// <inheritdoc/>
		protected override MongoRepository<TResult, TKey> CreateRepository(MongoDbWebhookTenantContext context)
			=> new MongoDbWebhookDeliveryResultRepository<TResult, TKey>(context, LoggerFactory?.CreateLogger<MongoDbWebhookDeliveryResultRepository<TResult, TKey>>());
	}
}
