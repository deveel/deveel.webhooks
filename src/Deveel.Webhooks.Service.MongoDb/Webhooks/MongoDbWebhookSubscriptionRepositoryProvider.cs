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

using Finbuckle.MultiTenant;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MongoDB.Bson;

namespace Deveel.Webhooks {
	/// <summary>
	/// A default implementation of the <see cref="IWebhookSubscriptionRepositoryProvider{TSubscription}"/>
	/// </summary>
	/// <typeparam name="TContext">
	/// The type of the <see cref="IMongoDbWebhookContext"/> to be used 
	/// to access the database.
	/// </typeparam>
	/// <typeparam name="TTenantInfo">
	/// The type of the tenant information.
	/// </typeparam>
	public class MongoDbWebhookSubscriptionRepositoryProvider<TContext> : MongoDbWebhookSubscriptionRepositoryProvider<TContext, MongoWebhookSubscription, ObjectId>,
		IWebhookSubscriptionRepositoryProvider<MongoWebhookSubscription, ObjectId>
		where TContext : class, IMongoDbWebhookContext {
		/// <inheritdoc/>
		public MongoDbWebhookSubscriptionRepositoryProvider(IRepositoryTenantResolver tenantResolver, ILoggerFactory? loggerFactory = null) 
			: base(tenantResolver, loggerFactory) {
		}

		/// <inheritdoc/>
		public new async Task<IWebhookSubscriptionRepository<MongoWebhookSubscription, ObjectId>> GetRepositoryAsync(string tenantId, CancellationToken cancellationToken = default) 
			=> (IWebhookSubscriptionRepository<MongoWebhookSubscription, ObjectId>) (await base.GetRepositoryAsync(tenantId));
	}
}
