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

namespace Deveel.Webhooks {
	public class MongoDbWebhookSubscriptionFactory<TSubscription> : IWebhookSubscriptionFactory<TSubscription> where TSubscription : MongoDbWebhookSubscription {
		protected virtual TSubscription CreateSubscription() {
			return Activator.CreateInstance<TSubscription>();
		}

		public virtual TSubscription Create(WebhookSubscriptionInfo subscriptionInfo) {
			var doc = CreateSubscription();
			doc.Name = subscriptionInfo.Name;
			doc.EventTypes = subscriptionInfo.EventTypes?.ToList();
			doc.DestinationUrl = subscriptionInfo.DestinationUrl.ToString();
			doc.RetryCount = subscriptionInfo.RetryCount;
			doc.Secret = subscriptionInfo.Secret;
			doc.LastStatusTime = subscriptionInfo.Active != null ?
					DateTimeOffset.UtcNow :
					DateTimeOffset.MinValue;
			doc.Headers = subscriptionInfo.Headers != null
					? new Dictionary<string, string>(subscriptionInfo.Headers)
					: null;
			doc.Filters = subscriptionInfo.Filters?.Select(MapFilter).ToList();
			doc.Metadata = subscriptionInfo.Metadata != null
					? new Dictionary<string, object>(subscriptionInfo.Metadata)
					: new Dictionary<string, object>();

			if (subscriptionInfo.Active != null) {
				doc.Status = subscriptionInfo.Active.Value ?
					WebhookSubscriptionStatus.Active :
					WebhookSubscriptionStatus.Suspended;
			} else {
				doc.Status = WebhookSubscriptionStatus.None;
			}

			return doc;
		}


		private MongoDbWebhookFilter MapFilter(IWebhookFilter filter)
			=> new MongoDbWebhookFilter {
				Expression = filter.Expression,
				Format = filter.Format
			};

		//IWebhookSubscription IWebhookSubscriptionFactory<IWebhookSubscription>.Create(WebhookSubscriptionInfo subscriptionInfo)
		//	=> Create(subscriptionInfo);
	}

	public class MongoDbWebhookSubscriptionFactory : MongoDbWebhookSubscriptionFactory<MongoDbWebhookSubscription> {

	}
}
