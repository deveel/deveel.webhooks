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
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides a contract for a store of webhook subscriptions.
	/// </summary>
	/// <typeparam name="TSubscription">
	/// The type of webhook subscription that is handled by the store.
	/// </typeparam>
	public interface IWebhookSubscriptionStore<TSubscription>
		where TSubscription : class, IWebhookSubscription {

		/// <summary>
		/// Gets the unique identifier of the subscription.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to get the identifier for.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns the unique identifier of the subscription,
		/// or <c>null</c> if the subscription is not stored.
		/// </returns>
		Task<string?> GetIdAsync(TSubscription subscription, CancellationToken cancellationToken);

		/// <summary>
		/// Creates a new subscription in the store.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to create.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns the unique identifier of the subscription created.
		/// </returns>
		Task<string> CreateAsync(TSubscription subscription, CancellationToken cancellationToken);

		/// <summary>
		/// Finds a subscription by its unique identifier.
		/// </summary>
		/// <param name="id">
		/// The unique identifier of the subscription to find.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns the instance of the subscription, or <c>null</c>
		/// if no subscription is found.
		/// </returns>
		Task<TSubscription?> FindByIdAsync(string id, CancellationToken cancellationToken);

		/// <summary>
		/// Deletes the given subscription from the store.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to delete.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns <c>true</c> if the subscription was deleted,
		/// otherwise <c>false</c>.
		/// </returns>
		Task<bool> DeleteAsync(TSubscription subscription, CancellationToken cancellationToken);

		/// <summary>
		/// Updates the given subscription in the store.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to update.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns <c>true</c> if the subscription was updated,
		/// otherwise <c>false</c>.
		/// </returns>
		Task<bool> UpdateAsync(TSubscription subscription, CancellationToken cancellationToken);

		/// <summary>
		/// Gets the total number of subscriptions in the store.
		/// </summary>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns the total number of subscriptions in the store.
		/// </returns>
		Task<int> CountAllAsync(CancellationToken cancellationToken);

		/// <summary>
		/// Gets a list of all the subscriptions in the store
		/// that are listening for the given event type.
		/// </summary>
		/// <param name="eventType">
		/// The event type to get the subscriptions for.
		/// </param>
		/// <param name="activeOnly">
		/// A flag indicating whether only active subscriptions
		/// should be returned.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a list of subscriptions that are listening
		/// for a given event type.
		/// </returns>
		Task<IList<TSubscription>> GetByEventTypeAsync(string eventType, bool activeOnly, CancellationToken cancellationToken);

		/// <summary>
		/// Sets the state of the given subscription.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to set the state for.
		/// </param>
		/// <param name="status">
		/// The new status of the subscription.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a task that completes when the status is set.
		/// </returns>
		Task SetStatusAsync(TSubscription subscription, WebhookSubscriptionStatus status, CancellationToken cancellationToken);
	}
}