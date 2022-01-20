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
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Webhooks.Storage {
	public sealed class MongoDbWebhookSubscriptionFactory : IWebhookSubscriptionFactory {
		public IWebhookSubscription Create(WebhookSubscriptionInfo subscriptionInfo) {
			var doc = new WebhookSubscriptionDocument {
				Name = subscriptionInfo.Name,
				EventTypes = subscriptionInfo.EventTypes?.ToList(),
				DestinationUrl = subscriptionInfo.DestinationUrl.ToString(),
				RetryCount = subscriptionInfo.RetryCount,
				Secret = subscriptionInfo.Secret,
				LastStatusTime = subscriptionInfo.Active != null ?
					DateTimeOffset.UtcNow :
					DateTimeOffset.MinValue,
				Headers = subscriptionInfo.Headers != null
					? new Dictionary<string, string>(subscriptionInfo.Headers)
					: null,
				Filters = subscriptionInfo.Filters?.Select(MapFilter).ToList(),
				Metadata = subscriptionInfo.Metadata != null
					? new Dictionary<string, object>(subscriptionInfo.Metadata)
					: new Dictionary<string, object>()
			};

			if (subscriptionInfo.Active != null) {
				doc.Status = subscriptionInfo.Active.Value ?
					WebhookSubscriptionStatus.Active :
					WebhookSubscriptionStatus.Suspended;
			} else {
				doc.Status = WebhookSubscriptionStatus.None;
			}

			return doc;
		}


		private WebhookFilterField MapFilter(IWebhookFilter filter)
			=> new WebhookFilterField {
				Expression = filter.Expression,
				Format = filter.Format
			};
	}
}
