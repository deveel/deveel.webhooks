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
	/// <typeparam name="TKey">
	/// The type of the key used to identify the subscription.
	/// </typeparam>
	public class WebhookSubscriptionResolver<TSubscription, TKey> : IWebhookSubscriptionResolver
		where TSubscription : class, IWebhookSubscription 
		where TKey : notnull {
		private readonly IWebhookSubscriptionRepository<TSubscription, TKey> repository;
		private readonly IWebhookSubscriptionCache? cache;
		private ILogger logger;

		/// <summary>
		/// Constructs a <see cref="WebhookSubscriptionResolver{TSubscription,TKey}"/>
		/// backed by a given store.
		/// </summary>
		/// <param name="repository">
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
			IWebhookSubscriptionRepository<TSubscription, TKey> repository,
			IWebhookSubscriptionCache? cache = null,
			ILogger<WebhookSubscriptionResolver<TSubscription, TKey>>? logger = null) {
			this.repository = repository;
			this.cache = cache;
			this.logger = logger ?? NullLogger<WebhookSubscriptionResolver<TSubscription, TKey>>.Instance;
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
		public async Task<IList<IWebhookSubscription>> ResolveSubscriptionsAsync(string eventType, bool activeOnly, CancellationToken cancellationToken) {
			try {
				var list = await GetCachedAsync(eventType, cancellationToken);

				if (list == null) {
					logger.LogTrace("No webhook subscriptions to event {EventType} were found in cache", eventType);

					var result = await repository.GetByEventTypeAsync(eventType, activeOnly, cancellationToken);
					list = result.Cast<IWebhookSubscription>().ToList();
				}

				return list;
			} catch (Exception ex) {
				logger.LogError(ex, "Error occurred while trying to resolve webhook subscriptions to event {EventType}", eventType);
				throw;
			}
		}

	}
}
