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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides an implementation of the <see cref="IWebhookDeliveryResultRepository{TResult}"/>
	/// that is backed by a MongoDB database.
	/// </summary>
	/// <typeparam name="TTenantInfo">
	/// The type of the tenant information.
	/// </typeparam>
    public class MongoDbWebhookDeliveryResultRepositoryProvider<TTenantInfo> : MongoDbWebhookDeliveryResultRepositoryProvider<TTenantInfo, MongoWebhookDeliveryResult>
		where TTenantInfo : class, ITenantInfo, new() {
		/// <summary>
		/// Constructs the store with the given store.
		/// </summary>
		/// <param name="tenantStore">
		/// The store that provides access to the tenants.
		/// </param>
		/// <param name="loggerFactory">
		/// A factory to create loggers.
		/// </param>
		public MongoDbWebhookDeliveryResultRepositoryProvider(IEnumerable<IMultiTenantStore<TTenantInfo>> tenantStore, ILoggerFactory? loggerFactory = null) 
			: base(tenantStore, loggerFactory) {
		}
	}
}
