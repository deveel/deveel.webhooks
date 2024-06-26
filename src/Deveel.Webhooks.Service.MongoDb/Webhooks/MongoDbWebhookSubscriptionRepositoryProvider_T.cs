﻿// Copyright 2022-2024 Antonello Provenzano
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
	/// <typeparam name="TContext">
	/// The type of the <see cref="IMongoDbWebhookContext"/> to be used
	/// to access the database.
	/// </typeparam>
	/// <typeparam name="TSubscription">
	/// The type of the subscription to store.
	/// </typeparam>
	/// <typeparam name="TTenantInfo">
	/// The type of the tenant information used to identify the MongoDB database
	/// </typeparam>
	public class MongoDbWebhookSubscriptionRepositoryProvider<TContext, TSubscription, TKey> :
		MongoRepositoryProvider<TContext, TSubscription, TKey>,
		IWebhookSubscriptionRepositoryProvider<TSubscription, TKey>, IDisposable
		where TContext : class, IMongoDbWebhookContext
		where TKey : notnull
		where TSubscription : MongoWebhookSubscription {

		/// <summary>
		/// Constructs a new instance of the repository provider.
		/// </summary>
		/// <param name="tenantStore">
		/// The store that provides access to the tenants information.
		/// </param>
		/// <param name="loggerFactory">
		/// An optional logger factory to be used to create loggers
		/// for the repositories.
		/// </param>
		public MongoDbWebhookSubscriptionRepositoryProvider(IRepositoryTenantResolver tenantResolver, ILoggerFactory? loggerFactory = null)
			: base(tenantResolver, loggerFactory) {
		}


		/// <inheritdoc/>
		protected override MongoRepository<TSubscription, TKey> CreateRepository(TContext context)
			=> new MongoDbWebhookSubscriptionRepository<TSubscription, TKey>(context, LoggerFactory?.CreateLogger<MongoDbWebhookSubscriptionRepository<TSubscription, TKey>>());

		/// <inheritdoc/>
		public new async Task<IWebhookSubscriptionRepository<TSubscription, TKey>> GetRepositoryAsync(string tenantId, CancellationToken cancellationToken = default) 
			=> (IWebhookSubscriptionRepository<TSubscription, TKey>)(await base.GetRepositoryAsync(tenantId));
	}
}
