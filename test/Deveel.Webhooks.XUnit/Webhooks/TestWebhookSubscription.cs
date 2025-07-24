// Copyright 2022-2025 Antonello Provenzano
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

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


using System;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Webhooks {
	public class TestWebhookSubscription : IWebhookSubscription {
		public string SubscriptionId { get; set; }

		public string TenantId { get; set; }

		public string Name { get; set; }

		public IEnumerable<string> EventTypes { get; set; }

		public string DestinationUrl { get; set; }

		public string Secret { get; set; }

		public WebhookSubscriptionStatus Status { get; set; }

		public int? RetryCount { get; set; }

		IEnumerable<IWebhookFilter> IWebhookSubscription.Filters => Filters.Cast<IWebhookFilter>();

		public IList<WebhookFilter> Filters { get; set; }

		public IDictionary<string, string> Headers { get; set; }

		public IDictionary<string, object> Properties { get; set; }

		public DateTimeOffset? CreatedAt { get; set; }

		public DateTimeOffset? UpdatedAt { get; set; }

		public string? Format { get; set; }
	}
}
