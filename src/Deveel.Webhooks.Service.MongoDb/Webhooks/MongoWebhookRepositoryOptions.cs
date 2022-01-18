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

namespace Deveel.Webhooks {
	public class MongoWebhookRepositoryOptions {
		public string Database { get; set; }

		public string ConnectionString { get; set; }

		public string Collection { get; set; }

		public string SslProtocol { get; set; }

		public string TenantId { get; set; }

		public bool HasTenant => !String.IsNullOrWhiteSpace(TenantId);
	}
}
