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
	public sealed class MongoDbWebhookOptions {
		public string DatabaseName { get; set; }

		public string SubscriptionsCollectionName { get; set; }

		public string WebhooksCollectionName { get; set; }

		public string ConnectionString { get; set; }

		public string TenantId { get; set; }

		public string TenantDatabaseFormat { get; set; } = "{tenant}_{database}";

		public string TenantCollectionFormat { get; set; } = "{tenant}_{collection}";

		public string TenantField { get; set; } = "TenantId";

		public MongoDbMultiTenancyHandling MultiTenantHandling { get; set; }
	}
}
