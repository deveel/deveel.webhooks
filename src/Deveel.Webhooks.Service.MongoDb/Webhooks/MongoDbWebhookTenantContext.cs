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
#if NET7_0_OR_GREATER
using Finbuckle.MultiTenant.Abstractions;
#endif

using MongoFramework;

namespace Deveel.Webhooks {
	/// <summary>
	/// Represents a multi-tenant MongoDB context that can be used to
	/// access to tenant-specific databases
	/// </summary>
	public class MongoDbWebhookTenantContext: MongoDbMultiTenantContext, IMongoDbWebhookContext {
		/// <summary>
		/// Constructs the context with the given options
		/// </summary>
		/// <param name="connection">
		/// The connection to the MongoDB server.
		/// </param>
		/// <param name="multiTenantContextAccessor">
		/// The information about the tenant that
		/// owns the context.
		/// </param>
		public MongoDbWebhookTenantContext(IMongoDbConnection<MongoDbWebhookTenantContext> connection, IMultiTenantContextAccessor multiTenantContextAccessor) 
			: base(connection, multiTenantContextAccessor) {
		}

		/// <inheritdoc/>
		protected override void OnConfigureMapping(MappingBuilder mappingBuilder) {
			mappingBuilder.Entity<MongoWebhookSubscription>();
			mappingBuilder.Entity<MongoWebhookDeliveryResult>();

			base.OnConfigureMapping(mappingBuilder);
		}
    }
}
