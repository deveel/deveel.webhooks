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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public class TestSubscriptionResolver : IWebhookSubscriptionResolver {
		private readonly IList<IWebhookSubscription> subscriptions = new List<IWebhookSubscription>();

		public void AddSubscription(IWebhookSubscription subscription) {
			subscriptions.Add(subscription);
		}


		public Task<IList<IWebhookSubscription>> ResolveSubscriptionsAsync(string eventType, bool activeOnly, CancellationToken cancellationToken) {
			var result = subscriptions.Where(x => (!activeOnly || x.Status == WebhookSubscriptionStatus.Active) &&
				(x.EventTypes?.Any(y => y == eventType) ?? false))
				.ToList();

			return Task.FromResult<IList<IWebhookSubscription>>(result);
		}

	}
}
