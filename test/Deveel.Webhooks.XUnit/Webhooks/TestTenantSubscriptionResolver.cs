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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public class TestTenantSubscriptionResolver : ITenantWebhookSubscriptionResolver {
		private readonly Dictionary<string, List<IWebhookSubscription>> subscriptions;

		public TestTenantSubscriptionResolver() {
			subscriptions = new Dictionary<string, List<IWebhookSubscription>>();
		}

		public void AddSubscription(IWebhookSubscription subscription) {
			if (!subscriptions.TryGetValue(subscription.TenantId!, out var list)) {
				list = new List<IWebhookSubscription>();
				subscriptions.Add(subscription.TenantId!, list);
			}

			list.Add(subscription);
		}

		public Task<IList<IWebhookSubscription>> ResolveSubscriptionsAsync(string tenantId, string eventType, bool activeOnly, CancellationToken cancellationToken) {
			if (String.IsNullOrWhiteSpace(tenantId))
				throw new ArgumentNullException(nameof(tenantId));

			if (!subscriptions.TryGetValue(tenantId, out var list)) {
				return Task.FromResult<IList<IWebhookSubscription>>(new List<IWebhookSubscription>());
			}

			var result = list.Where(x => (!activeOnly || x.Status == WebhookSubscriptionStatus.Active) &&
				(x.EventTypes?.Any(y => y == eventType) ?? false))
				.ToList();

			return Task.FromResult<IList<IWebhookSubscription>>(result);
		}
	}
}
