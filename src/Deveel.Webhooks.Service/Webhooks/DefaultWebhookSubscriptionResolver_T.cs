// Copyright 2022-2023 Deveel
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

using Deveel.Webhooks.Caching;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Deveel.Webhooks {
	public class DefaultWebhookSubscriptionResolver<TSubscription> : IWebhookSubscriptionResolver
		where TSubscription : class, IWebhookSubscription {
		private readonly IWebhookSubscriptionStoreProvider<TSubscription> storeProvider;
		private readonly IWebhookSubscriptionCache cache;
		private ILogger logger;

		public DefaultWebhookSubscriptionResolver(
			IWebhookSubscriptionStoreProvider<TSubscription> storeProvider,
			IWebhookSubscriptionCache cache = null,
			ILogger<DefaultWebhookSubscriptionResolver<TSubscription>> logger = null) {
			this.storeProvider = storeProvider;
			this.cache = cache;
			this.logger = logger ?? NullLogger<DefaultWebhookSubscriptionResolver<TSubscription>>.Instance;
		}

		private async Task<IList<IWebhookSubscription>> GetCachedAsync(string tenantId, string eventType, CancellationToken cancellationToken) {
			try {
				if (cache == null) {
					logger.LogTrace("No webhook subscriptions cache was set");
					return null;
				}

				logger.LogTrace("Trying to retrieve webhook subscriptions to event {EventType} of tenant {TenantId}", eventType, tenantId);

				return await cache.GetByEventTypeAsync(tenantId, eventType, cancellationToken);
			} catch (Exception ex) {
				logger.LogError(ex, "Could not get the cached webhook subscriptions to event {EventType} of tenant {TenantId}", eventType, tenantId);
				return null;
			}
		}

		public async Task<IList<IWebhookSubscription>> ResolveSubscriptionsAsync(string tenantId, string eventType, bool activeOnly, CancellationToken cancellationToken) {
			try {
				var list = await GetCachedAsync(tenantId, eventType, cancellationToken);

				if (list == null) {
					logger.LogTrace("No webhook subscriptions to event {EventType} of tenant {TenantId} were found in cache", eventType, tenantId);

					var result = await storeProvider.GetByEventTypeAsync(tenantId, eventType, activeOnly, cancellationToken);
					list = result.Cast<IWebhookSubscription>().ToList();
				}

				return list;
			} catch (Exception ex) {
				logger.LogError(ex, "Error occurred while trying to resolve webhook subscriptions to event {EventType} of tenant {TenantId}", eventType, tenantId);
				throw;
			}
		}
	}
}
