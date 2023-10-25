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

using Deveel.Data;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides a contract for a store of webhook subscriptions.
	/// </summary>
	/// <typeparam name="TSubscription">
	/// The type of webhook subscription that is handled by the store.
	/// </typeparam>
	public interface IWebhookSubscriptionRepository<TSubscription> : IRepository<TSubscription>
		where TSubscription : class, IWebhookSubscription {
		Task<string?> GetDestinationUrlAsync(TSubscription subscription, CancellationToken cancellationToken = default);

		Task SetDestinationUrlAsync(TSubscription subscription, string url, CancellationToken cancellationToken = default);

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
		Task<IList<TSubscription>> GetByEventTypeAsync(string eventType, bool? activeOnly, CancellationToken cancellationToken = default);

		Task<WebhookSubscriptionStatus> GetStatusAsync(TSubscription subscription, CancellationToken cancellationToken = default);

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
		Task SetStatusAsync(TSubscription subscription, WebhookSubscriptionStatus status, CancellationToken cancellationToken = default);

		Task<string[]> GetEventTypesAsync(TSubscription subscription, CancellationToken cancellationToken = default);

		Task AddEventTypesAsync(TSubscription subscription, string[] eventTypes, CancellationToken cancellationToken = default);

		Task RemoveEventTypesAsync(TSubscription subscription, string[] eventTypes, CancellationToken cancellationToken = default);

		Task<IDictionary<string, string>> GetHeadersAsync(TSubscription subscription, CancellationToken cancellationToken = default);

		Task AddHeadersAsync(TSubscription subscription, IDictionary<string, string> headers, CancellationToken cancellationToken = default);

		Task RemoveHeadersAsync(TSubscription subscription, string[] headerNames, CancellationToken cancellationToken = default);
	}
}