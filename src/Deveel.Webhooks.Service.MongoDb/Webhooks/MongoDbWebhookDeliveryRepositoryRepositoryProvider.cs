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

using Deveel.Data;

using Finbuckle.MultiTenant;

using Microsoft.Extensions.Logging;

using SharpCompress.Compressors.PPMd;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides an implementation of the <see cref="IWebhookDeliveryResultRepositoryProvider{TResult}"/>
	/// that is resolving instances of <see cref="MongoDbWebhookDeliveryResultRepository{TResult}"/> based
	/// on the tenant identifier.
	/// </summary>
	/// <typeparam name="TTenantInfo">
	/// The type of tenant that owns the connection to the MongoDB database
	/// </typeparam>
	/// <typeparam name="TResult">
	/// The type of the result that is stored in the database.
	/// </typeparam>
	public class MongoDbWebhookDeliveryResultRepositoryProvider<TTenantInfo, TResult> : MongoRepositoryProvider<MongoDbWebhookTenantContext, TResult, TTenantInfo>, 
		IWebhookDeliveryResultRepositoryProvider<TResult>
		where TTenantInfo : class, ITenantInfo, new()
		where TResult : MongoWebhookDeliveryResult {
        /// <summary>
        /// Constructs the provider with the given tenant store.
        /// </summary>
        /// <param name="tenantStore">
        /// The store that is used to resolve the tenant information.
        /// </param>
		/// <param name="loggerFactory">
		/// An object that is used to create the logger for the repositories
		/// created by this provider.
		/// </param>
        public MongoDbWebhookDeliveryResultRepositoryProvider(
			IEnumerable<IMultiTenantStore<TTenantInfo>> tenantStore, 
			ILoggerFactory? loggerFactory = null)
			: base(tenantStore, loggerFactory) {
		}

        /// <inheritdoc/>
        public new async Task<IWebhookDeliveryResultRepository<TResult>> GetRepositoryAsync(string tenantId, CancellationToken cancellationToken = default) {
			return (IWebhookDeliveryResultRepository<TResult>) await base.GetRepositoryAsync(tenantId, cancellationToken);
		}

		/// <inheritdoc/>
		protected override MongoRepository<TResult> CreateRepository(MongoDbWebhookTenantContext context)
			=> new MongoDbWebhookDeliveryResultRepository<TResult>(context, LoggerFactory?.CreateLogger<MongoDbWebhookDeliveryResultRepository<TResult>>());
	}
}
