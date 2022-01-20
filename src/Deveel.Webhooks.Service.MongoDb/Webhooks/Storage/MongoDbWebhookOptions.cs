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

namespace Deveel.Webhooks.Storage {
	/// <summary>
	/// The configuration options for the MongoDB data layer
	/// of the webhook management.
	/// </summary>
	public sealed class MongoDbWebhookOptions {
		/// <summary>
		/// Gets or sets the name of the database.
		/// </summary>
		public string DatabaseName { get; set; }

		/// <summary>
		/// Gets or sets the name of the webhook subscriptions collection
		/// </summary>
		public string SubscriptionsCollectionName { get; set; }

		/// <summary>
		/// Gets or sets the name of the webhook logging collection
		/// </summary>
		public string WebhooksCollectionName { get; set; }

		/// <summary>
		/// Gets or sets the connection string to the MongoDB server
		/// </summary>
		public string ConnectionString { get; set; }

		/// <summary>
		/// Gets or sets the identifier of the tenant
		/// </summary>
		public string TenantId { get; set; }

		public string TenantDatabaseFormat { get; set; } = "{tenant}_{database}";

		public string TenantCollectionFormat { get; set; } = "{tenant}_{collection}";

		public string TenantField { get; set; } = "TenantId";

		/// <summary>
		/// Gets or sets the type of handling of the multi-tenant scenarios of usage
		/// </summary>
		public MongoDbMultiTenancyHandling MultiTenantHandling { get; set; } = MongoDbMultiTenancyHandling.TenantField;
	}
}
