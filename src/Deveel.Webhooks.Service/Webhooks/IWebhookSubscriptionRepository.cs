// Copyright 2022-2025 Antonello Provenzano
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

using Deveel.Data;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides a contract for a store of webhook subscriptions.
	/// </summary>
	/// <typeparam name="TSubscription">
	/// The type of webhook subscription that is handled by the store.
	/// </typeparam>
	/// <typeparam name="TKey">
	/// The type of the key used to identify the subscription.
	/// </typeparam>
	public interface IWebhookSubscriptionRepository<TSubscription, TKey> : IRepository<TSubscription, TKey>
		where TSubscription : class, IWebhookSubscription
		where TKey : notnull {
		/// <summary>
		/// Gets the URL of the destination where to deliver the
		/// webhook events for the given subscription.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to get the destination URL for.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns the URL of the destination where to deliver the
		/// webhook events for the given subscription, or <c>null</c>
		/// if the subscription has no destination URL set.
		/// </returns>
		Task<string?> GetDestinationUrlAsync(TSubscription subscription, CancellationToken cancellationToken = default);

		/// <summary>
		/// Sets the URL of the destination where to deliver the
		/// webhook events for the given subscription.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to set the destination URL for.
		/// </param>
		/// <param name="url">
		/// The URL of the destination where to deliver the webhook events.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a task that completes when the destination URL is set.
		/// </returns>
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

		/// <summary>
		/// Gets the current status of the given subscription.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to get the status for.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns the current status of the subscription.
		/// </returns>
		Task<WebhookSubscriptionStatus> GetStatusAsync(TSubscription subscription, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets the secret that is used to sign the webhooks
		/// delivered to the given subscription.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to get the secret for.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns the secret that is used to sign the webhooks
		/// delivered to the given subscription, or <c>null</c>
		/// if the subscription has no secret set.
		/// </returns>
		Task<string?> GetSecretAsync(TSubscription subscription, CancellationToken cancellationToken = default);

		/// <summary>
		/// Sets the secret that is used to sign the webhooks
		/// to be delivered to the given subscription.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to set the secret for.
		/// </param>
		/// <param name="secret">
		/// The secret to set for the subscription, or
		/// <c>null</c> to remove the secret from the subscription.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a task that completes when the secret is set.
		/// </returns>
		Task SetSecretAsync(TSubscription subscription, string? secret, CancellationToken cancellationToken = default);

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

		/// <summary>
		/// Gets the list of event types that the given subscription
		/// is listening for.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to get the event types for.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns an array of event types that the subscription is
		/// listening for.
		/// </returns>
		Task<string[]> GetEventTypesAsync(TSubscription subscription, CancellationToken cancellationToken = default);

		/// <summary>
		/// Adds the given set of event types to the list of the
		/// ones that the given subscription is listening for.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to add the event types to.
		/// </param>
		/// <param name="eventTypes">
		/// The list of the new event types to add to the subscription.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a task that completes when the event types are added.
		/// </returns>
		Task AddEventTypesAsync(TSubscription subscription, string[] eventTypes, CancellationToken cancellationToken = default);

		/// <summary>
		/// Removes the given set of event types from the list of the
		/// ones that the given subscription is listening for.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to remove the event types from.
		/// </param>
		/// <param name="eventTypes">
		/// The list of the event types to remove from the subscription.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a task that completes when the event types are removed.
		/// </returns>
		Task RemoveEventTypesAsync(TSubscription subscription, string[] eventTypes, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets the list of the headers that are set for the given subscription,
		/// to be sent to the destination URL together with the webhook.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to get the headers for.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a dictionary of the headers that are set for the subscription.
		/// </returns>
		Task<IDictionary<string, string>> GetHeadersAsync(TSubscription subscription, CancellationToken cancellationToken = default);

		/// <summary>
		/// Adds new headers to the list of the ones that are set to be sent
		/// to the destination URL together with the webhook.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to add the headers to.
		/// </param>
		/// <param name="headers">
		/// The new headers to add to the subscription.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a task that completes when the headers are added.
		/// </returns>
		Task AddHeadersAsync(TSubscription subscription, IDictionary<string, string> headers, CancellationToken cancellationToken = default);

		/// <summary>
		/// Removes the given set of headers from the list of the ones that
		/// will be sent to the destination URL together with the webhook.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to remove the headers from.
		/// </param>
		/// <param name="headerNames">
		/// The list of the names of the headers to remove from the subscription.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a task that completes when the headers are removed.
		/// </returns>
		Task RemoveHeadersAsync(TSubscription subscription, string[] headerNames, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets the list of the properties that are set for the given subscription,
		/// used to configure the behaviour of the webhook notification.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to get the properties for.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a dictionary of the properties that are set for the subscription.
		/// </returns>
		Task<IDictionary<string, object>> GetPropertiesAsync(TSubscription subscription, CancellationToken cancellationToken = default);

		/// <summary>
		/// Adds new properties to the list of the ones that are set to configure
		/// the behaviour of the webhook notification.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to add the properties to.
		/// </param>
		/// <param name="properties">
		/// The new properties to add to the subscription.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a task that completes when the properties are added.
		/// </returns>
		Task AddPropertiesAsync(TSubscription subscription, IDictionary<string, object> properties, CancellationToken cancellationToken = default);

		/// <summary>
		/// Removes the given set of properties from the list of the ones that
		/// are used to configure the behaviour of the webhook notification.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to remove the properties from.
		/// </param>
		/// <param name="propertyNames">
		/// The list of the names of the properties to remove from the subscription.
		/// </param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task RemovePropertiesAsync(TSubscription subscription, string[] propertyNames, CancellationToken cancellationToken = default);
	}
}