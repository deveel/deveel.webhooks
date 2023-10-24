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

namespace Deveel.Webhooks {
	/// <summary>
	/// A service that implements the <see cref="IWebhookSubscriptionRepositoryProvider{TSubscription}"/>
	/// contract to resolve a store for a given tenant.
	/// </summary>
	/// <typeparam name="TTenantInfo">
	/// The type of the tenant information used to identify the MongoDB database
	/// </typeparam>
	/// <typeparam name="TSubscription">
	/// The type of the subscription to store.
	/// </typeparam>
	public class MongoDbWebhookSubscriptionRepositoryProvider<TContext, TSubscription, TTenantInfo> :
		MongoRepositoryProvider<TContext, TSubscription, TTenantInfo>,
		IWebhookSubscriptionRepositoryProvider<TSubscription>, IDisposable
		where TContext : class, IMongoDbWebhookContext
		where TTenantInfo : class, ITenantInfo, new()
		where TSubscription : MongoWebhookSubscription {

		/// <summary>
		/// Constructs a new instance of the store provider.
		/// </summary>
		/// <param name="tenantStore">
		/// The store that provides access to the tenants information.
		/// </param>
		public MongoDbWebhookSubscriptionRepositoryProvider(IEnumerable<IMultiTenantStore<TTenantInfo>> tenantStore, ILoggerFactory? loggerFactory = null)
			: base(tenantStore, loggerFactory) {
		}


		/// <inheritdoc/>
		protected override MongoRepository<TSubscription> CreateRepository(TContext context)
			=> new MongoDbWebhookSubscriptionRepository<TSubscription>(context, LoggerFactory?.CreateLogger<MongoDbWebhookSubscriptionRepository<TSubscription>>());

		/// <inheritdoc/>
		public new async Task<IWebhookSubscriptionRepository<TSubscription>> GetRepositoryAsync(string tenantId, CancellationToken cancellationToken = default) 
			=> (IWebhookSubscriptionRepository<TSubscription>)(await base.GetRepositoryAsync(tenantId));
	}
}
