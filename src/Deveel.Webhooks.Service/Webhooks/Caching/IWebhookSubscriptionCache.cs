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

namespace Deveel.Webhooks.Caching {
	/// <summary>
	/// A cache of webhook subscriptions that optimizes the
	/// read access to the entities
	/// </summary>
	public interface IWebhookSubscriptionCache {
		/// <summary>
		/// Gets a single subscription by its identifier
		/// </summary>
		/// <param name="tenantId">
		/// The identifier of the tenant that owns the subscription
		/// </param>
		/// <param name="id">
		/// The unique identifier of the subscription
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation
		/// </param>
		/// <returns>
		/// Returns the subscription if found, or <c>null</c> otherwise
		/// </returns>
		Task<IWebhookSubscription?> GetByIdAsync(string tenantId, string id, CancellationToken cancellationToken);

		/// <summary>
		/// Attempts to get a list of subscriptions that are
		/// listening for the given event type
		/// </summary>
		/// <param name="tenantId">
		/// The identifier of the tenant that owns the subscriptions
		/// </param>
		/// <param name="eventType">
		/// The event type that the subscriptions are listening for
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation
		/// </param>
		/// <returns>
		/// Returns a list of subscriptions that are listening for the given event type
		/// </returns>
		Task<IList<IWebhookSubscription>> GetByEventTypeAsync(string tenantId, string eventType, CancellationToken cancellationToken);

		/// <summary>
		/// Evicts from the cache all the subscriptions that are 
		/// listening for the given event type
		/// </summary>
		/// <param name="tenantId">
		/// The identifier of the tenant that owns the subscriptions
		/// </param>
		/// <param name="eventType">
		/// The event type that the subscriptions are listening for
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation
		/// </param>
		/// <returns>
		/// Returns a task that completes when the operation is done
		/// </returns>
		Task EvictByEventTypeAsync(string tenantId, string eventType, CancellationToken cancellationToken);

		/// <summary>
		/// Sets the given subscription in the cache
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to cache
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation
		/// </param>
		/// <returns>
		/// Returns a task that completes when the operation is done
		/// </returns>
		Task SetAsync(IWebhookSubscription subscription, CancellationToken cancellationToken);

		/// <summary>
		/// Removes the given subscription from the cache
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to remove
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation
		/// </param>
		/// <returns>
		/// Returns a task that completes when the operation is done
		/// </returns>
		Task RemoveAsync(IWebhookSubscription subscription, CancellationToken cancellationToken);
	}
}
