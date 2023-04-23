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

using Deveel.Webhooks.Caching;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Deveel.Webhooks {
	/// <summary>
	/// A default implementation of <see cref="IWebhookSubscriptionResolver"/> that
	/// uses a registered store provider to retrieve the subscriptions.
	/// </summary>
	/// <typeparam name="TSubscription">
	/// The type of the subscription to be resolved.
	/// </typeparam>
	public class WebhookSubscriptionResolver<TSubscription> : IWebhookSubscriptionResolver
		where TSubscription : class, IWebhookSubscription {
		private readonly IWebhookSubscriptionStoreProvider<TSubscription> storeProvider;
		private readonly IWebhookSubscriptionCache? cache;
		private ILogger logger;

		/// <summary>
		/// Constructs a <see cref="WebhookSubscriptionResolver{TSubscription}"/>
		/// backed by a given store provider.
		/// </summary>
		/// <param name="storeProvider">
		/// The provider of the store to be used to retrieve the subscriptions.
		/// </param>
		/// <param name="cache">
		/// An optional cache of the subscriptions to be used to speed up the
		/// resolution process.
		/// </param>
		/// <param name="logger">
		/// An optional logger to be used to log the operations.
		/// </param>
		public WebhookSubscriptionResolver(
			IWebhookSubscriptionStoreProvider<TSubscription> storeProvider,
			IWebhookSubscriptionCache? cache = null,
			ILogger<WebhookSubscriptionResolver<TSubscription>>? logger = null) {
			this.storeProvider = storeProvider;
			this.cache = cache;
			this.logger = logger ?? NullLogger<WebhookSubscriptionResolver<TSubscription>>.Instance;
		}

		private async Task<IList<IWebhookSubscription>?> GetCachedAsync(string tenantId, string eventType, CancellationToken cancellationToken) {
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

		/// <inheritdoc/>
		public async Task<IList<IWebhookSubscription>> ResolveSubscriptionsAsync(string tenantId, string eventType, bool activeOnly, CancellationToken cancellationToken) {
			try {
				var list = await GetCachedAsync(tenantId, eventType, cancellationToken);

				if (list == null) {
					logger.LogTrace("No webhook subscriptions to event {EventType} of tenant {TenantId} were found in cache", eventType, tenantId);

					var store = storeProvider.GetTenantStore(tenantId);
					var result = await store.GetByEventTypeAsync(eventType, activeOnly, cancellationToken);
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
