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
using System.Threading;
using System.Threading.Tasks;

using Deveel.Webhooks.Storage;

namespace Deveel.Webhooks {
	public class DefaultWebhookSubscriptionResolver<TSubscription> : IWebhookSubscriptionResolver
		where TSubscription : class, IWebhookSubscription {
		private readonly IWebhookSubscriptionStoreProvider<TSubscription> storeProvider;

		public DefaultWebhookSubscriptionResolver(IWebhookSubscriptionStoreProvider<TSubscription> storeProvider) {
			this.storeProvider = storeProvider;
		}

		public async Task<IList<IWebhookSubscription>> ResolveSubscriptionsAsync(string tenantId, string eventType, bool activeOnly, CancellationToken cancellationToken) {
			var result = await storeProvider.GetByEventTypeAsync(tenantId, eventType, activeOnly, cancellationToken);
			return result.Cast<IWebhookSubscription>().ToList();
		}
	}
}
