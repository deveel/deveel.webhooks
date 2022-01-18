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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Webhooks {
	public sealed class WebhookNotificationResult : IEnumerable<WebhookDeliveryResult> {
		private readonly List<WebhookDeliveryResult> deliveryResults;

		public WebhookNotificationResult() {
			deliveryResults = new List<WebhookDeliveryResult>();
		}

		public void AddDelivery(WebhookDeliveryResult result) {
			lock (this) {
				deliveryResults.Add(result);
			}
		}

		public bool HasSuccessful => Successful?.Any() ?? false;

		public IEnumerable<WebhookDeliveryResult> Successful
			=> deliveryResults.Where(x => x.Successful);

		public bool HasFailed => Failed?.Any() ?? false;

		public IEnumerable<WebhookDeliveryResult> Failed
			=> deliveryResults.Where(x => !x.Successful);

		public bool IsEmpty => deliveryResults.Count == 0;

		public WebhookDeliveryResult this[string subscriptionId] => deliveryResults.ToDictionary(x => x.Webhook.SubscriptionId, y => y)[subscriptionId];

		public IEnumerator<WebhookDeliveryResult> GetEnumerator() => deliveryResults.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
