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
	/// uses a registered store to retrieve the subscriptions.
	/// </summary>
	/// <typeparam name="TSubscription">
	/// The type of the subscription to be resolved.
	/// </typeparam>
	public class WebhookSubscriptionResolver<TSubscription> : IWebhookSubscriptionResolver
		where TSubscription : class, IWebhookSubscription {
		private readonly IWebhookSubscriptionStore<TSubscription> store;
		private readonly IWebhookSubscriptionCache? cache;
		private ILogger logger;

		/// <summary>
		/// Constructs a <see cref="WebhookSubscriptionResolver{TSubscription}"/>
		/// backed by a given store.
		/// </summary>
		/// <param name="store">
		/// The store to be used to retrieve the subscriptions.
		/// </param>
		/// <param name="cache">
		/// An optional cache of the subscriptions to be used to speed up the
		/// resolution process.
		/// </param>
		/// <param name="logger">
		/// An optional logger to be used to log the operations.
		/// </param>
		public WebhookSubscriptionResolver(
			IWebhookSubscriptionStore<TSubscription> store,
			IWebhookSubscriptionCache? cache = null,
			ILogger<WebhookSubscriptionResolver<TSubscription>>? logger = null) {
			this.store = store;
			this.cache = cache;
			this.logger = logger ?? NullLogger<WebhookSubscriptionResolver<TSubscription>>.Instance;
		}

		private async Task<IList<IWebhookSubscription>?> GetCachedAsync(string eventType, CancellationToken cancellationToken) {
			try {
				if (cache == null) {
					logger.LogTrace("No webhook subscriptions cache was set");
					return null;
				}

				logger.LogTrace("Trying to retrieve webhook subscriptions to event {EventType}", eventType);

				return await cache.GetByEventTypeAsync(eventType, cancellationToken);
			} catch (Exception ex) {
				logger.LogError(ex, "Could not get the cached webhook subscriptions to event {EventType}", eventType);
				return null;
			}
		}

		/// <inheritdoc/>
		public async Task<IList<IWebhookSubscription>> ResolveSubscriptionsAsync(IEventInfo eventInfo, bool activeOnly, CancellationToken cancellationToken) {
			try {
				// TODO: have a cache key builder
				var list = await GetCachedAsync(eventInfo.EventType, cancellationToken);

				if (list == null) {
					logger.LogTrace("No webhook subscriptions to event {EventType} of tenant {TenantId} were found in cache", eventInfo.EventType);

					// TODO: make a more advanced query to use
					//       also the properties of the event
					var result = await store.GetByEventTypeAsync(eventInfo.EventType, activeOnly, cancellationToken);
					list = result.Cast<IWebhookSubscription>().ToList();
				}

				return list;
			} catch (Exception ex) {
				logger.LogError(ex, "Error occurred while trying to resolve webhook subscriptions to event {EventType}", eventInfo.EventType);
				throw new WebhookServiceException("Could not resolve the subscriptions: see the inner exception for details", ex);
			}
		}

	}
}
