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

using Deveel.Data;

using Microsoft.Extensions.Logging;

namespace Deveel.Webhooks {
	/// <summary>
	/// A manager of webhook subscriptions that provides a set of operations
	/// for the handling of entities in a store.
	/// </summary>
	/// <typeparam name="TSubscription">
	/// The type of the subscription handled by the manager.
	/// </typeparam>
	public class WebhookSubscriptionManager<TSubscription, TKey> : EntityManager<TSubscription, TKey>
		where TKey : notnull
		where TSubscription : class, IWebhookSubscription {
		/// <summary>
		/// Creates a new instance of the manager wrapping a given store
		/// of webhook subscriptions entities
		/// </summary>
		/// <param name="subscriptionStore">
		/// The store of webhook subscriptions entities.
		/// </param>
		/// <param name="validator">
		/// An optional service to be used to validate webhook subscriptions 
		/// before creating or updating them.
		/// </param>
		/// <param name="services">
		/// A service provider used to resolve services used by the manager.
		/// </param>
		/// <param name="logger">
		/// A logger to be used to log messages informing on the operations
		/// of the manager.
		/// </param>
		//TODO: add an Error Factory for Webhook Subscriptions
		public WebhookSubscriptionManager(
			IWebhookSubscriptionRepository<TSubscription, TKey> subscriptionStore,
			IWebhookSubscriptionValidator<TSubscription, TKey>? validator = null,
			IServiceProvider? services = null,
			ILogger<WebhookSubscriptionManager<TSubscription, TKey>>? logger = null)
			: base(subscriptionStore, validator, services: services) {
		}

		/// <summary>
		/// When the store supports queries, this gets a queryable
		/// object used to query the subscriptions.
		/// </summary>
		public IQueryable<TSubscription> Subscriptions => base.Entities;

		/// <summary>
		/// Gets an instance of the repository that implements 
		/// the webhook subscriptions operations.
		/// </summary>
		protected virtual IWebhookSubscriptionRepository<TSubscription, TKey> SubscriptionRepository {
			get {
				ThrowIfDisposed();
				return (IWebhookSubscriptionRepository<TSubscription, TKey>) base.Repository;
			}
		}

		// TODO: fix this in Deveel Repository, to configure if to 
		//       compare the two entities and disable the comparison by default...
		/// <inheritdoc/>
		protected override bool AreEqual(TSubscription existing, TSubscription other) => false;

		/// <inheritdoc/>
		protected override IOperationError OperationError(string errorCode, string? message = null) {
			errorCode = errorCode switch {
				EntityErrorCodes.NotValid => WebhookSubscriptionErrorCodes.SubscriptionInvalid,
				EntityErrorCodes.NotFound => WebhookSubscriptionErrorCodes.SubscriptionNotFound,
				EntityErrorCodes.UnknownError => WebhookSubscriptionErrorCodes.UnknownError,
				_ => errorCode
			};

			return base.OperationError(errorCode, message);
		}

		/// <summary>
		/// Gets the current status of a given subscription.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to get the status.
		/// </param>
		/// <param name="cancellationToken">
		/// A token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns the current status of the subscription.
		/// </returns>
		/// <exception cref="WebhookException">
		/// Thrown if the status of the subscription could not be 
		/// retrieved because of an unhanded exception.
		/// </exception>
		public virtual Task<WebhookSubscriptionStatus> GetStatusAsync(TSubscription subscription, CancellationToken? cancellationToken = null) {
			try {
				return SubscriptionRepository.GetStatusAsync(subscription, GetCancellationToken(cancellationToken));
			} catch (Exception ex) {
				Logger.LogUnknownSubscriptionError(ex, subscription.SubscriptionId);
				throw new WebhookException("Error while trying to get the current status of the subscription", ex);
			}
		}

		/// <summary>
		/// Gets the list of subscriptions that are listening for a given 
		/// event type.
		/// </summary>
		/// <param name="eventType">
		/// The event type to get the subscriptions for.
		/// </param>
		/// <param name="activeOnly">
		/// Indicates whether only active subscriptions should be returned.
		/// </param>
		/// <returns>
		/// Returns a list of subscriptions that are listening for a given 
		/// event type.
		/// </returns>
		/// <exception cref="WebhookException"></exception>
		public virtual async Task<IList<TSubscription>> GetByEventTypeAsync(string eventType, bool? activeOnly) {
			try {
				return await SubscriptionRepository.GetByEventTypeAsync(eventType, activeOnly, CancellationToken);
			} catch (Exception ex) {
				Logger.LogUnknownError(ex);
				throw new WebhookException($"Could not get the subscriptions for event type '{eventType}'", ex);
			}
		}

        /// <summary>
        /// Sets the new status of a subscription.
        /// </summary>
        /// <param name="subscription">
        /// The subscription to change the status.
        /// </param>
        /// <param name="status">
        /// The new status to set.
        /// </param>
		/// <param name="cancellationToken">
		/// A token used to cancel the operation.
		/// </param>
        /// <returns>
        /// Returns <c>true</c> if the status was changed, or <c>false</c>
        /// </returns>
        /// <exception cref="WebhookException">
        /// Thrown if the status of the subscription could not be changed because of
        /// an unhanded exception.
        /// </exception>
        public async Task<OperationResult> SetStatusAsync(TSubscription subscription, WebhookSubscriptionStatus status, CancellationToken? cancellationToken = null) {
            try {
				var currentStatus = await SubscriptionRepository.GetStatusAsync(subscription, CancellationToken);

                if (currentStatus == status) {
                    Logger.LogTrace("The subscription {SubscriptionId} is already {Status}", subscription.SubscriptionId, status);
					return OperationResult.NotChanged;
                }

                await SubscriptionRepository.SetStatusAsync(subscription, status, CancellationToken);

                Logger.LogInformation("The status of subscription {SubscriptionId} was changed to {Status}", subscription.SubscriptionId, status);

				return await UpdateAsync(subscription);
            } catch (Exception ex) {
				Logger.LogUnknownSubscriptionError(ex, subscription.SubscriptionId);
				return Fail(WebhookSubscriptionErrorCodes.UnknownError, "Unhandled error while changing the status");
            }
        }

        /// <summary>
        /// Disables a webhook subscription by setting its status to <see cref="WebhookSubscriptionStatus.Suspended"/>.
        /// </summary>
        /// <param name="subscription">
        /// The instance of the webhook subscription to disable.
        /// </param>
		/// <param name="cancellationToken">
		/// A token used to cancel the operation.
		/// </param>
        /// <returns>
        /// Returns <c>true</c> if the subscription was disabled, or <c>false</c>
        /// if the subscription was already disabled.
        /// </returns>
        /// <seealso cref="SetStatusAsync(TSubscription, WebhookSubscriptionStatus, CancellationToken?)"/>
        public virtual Task<OperationResult> DisableAsync(TSubscription subscription, CancellationToken? cancellationToken = null)
			=> SetStatusAsync(subscription, WebhookSubscriptionStatus.Suspended, cancellationToken);

		/// <summary>
		/// Enables a webhook subscription by setting its status to <see cref="WebhookSubscriptionStatus.Active"/>.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the webhook subscription to enable.
		/// </param>
		/// <param name="cancellationToken">
		/// A token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns <c>true</c> if the subscription was enabled, or <c>false</c>
		/// if the subscription was already enabled.
		/// </returns>
		public virtual Task<OperationResult> EnableAsync(TSubscription subscription, CancellationToken? cancellationToken = null) 
			=> SetStatusAsync(subscription, WebhookSubscriptionStatus.Active, cancellationToken);

		/// <summary>
		/// Sets the event types that a subscription is listening for.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to set the event types
		/// that it is listening for.
		/// </param>
		/// <param name="eventTypes">
		/// The list of event types that the subscription should listen for.
		/// </param>
		/// <param name="cancellationToken">
		/// A token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns an object that represents the result of the operation.
		/// </returns>
		public virtual async Task<OperationResult> SetEventTypesAsync(TSubscription subscription, string[] eventTypes, CancellationToken? cancellationToken = null) {
			ThrowIfDisposed();

			ArgumentNullException.ThrowIfNull(subscription, nameof(subscription));
			ArgumentNullException.ThrowIfNull(eventTypes, nameof(eventTypes));

			try {
				eventTypes = eventTypes.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

				var existing = await SubscriptionRepository.GetEventTypesAsync(subscription, CancellationToken);

				var toAdd = eventTypes.Except(existing, StringComparer.OrdinalIgnoreCase).ToArray();
				var toRemove = existing.Except(eventTypes, StringComparer.OrdinalIgnoreCase).ToArray();

				if (toAdd.Length == 0 && toRemove.Length == 0)
					return OperationResult.NotChanged;

				if (toAdd.Length > 0)
					await SubscriptionRepository.AddEventTypesAsync(subscription, toAdd, CancellationToken);
				if (toRemove.Length > 0)
					await SubscriptionRepository.RemoveEventTypesAsync(subscription, toRemove, CancellationToken);

				return await UpdateAsync(subscription);
			} catch (Exception ex) {
				Logger.LogUnknownSubscriptionError(ex, subscription.SubscriptionId);
				return Fail(WebhookSubscriptionErrorCodes.UnknownError, "Unhandled error while setting the event types");
			}
		}

		/// <summary>
		/// Sets the headers that are sent to the destination URL of 
		/// a subscription together with the webhook notification.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to set the headers.
		/// </param>
		/// <param name="headers">
		/// The list of headers to set.
		/// </param>
		/// <param name="cancellationToken">
		/// A token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns an object that represents the result of the operation.
		/// </returns>
		public async Task<OperationResult> SetHeadersAsync(TSubscription subscription, IDictionary<string, string> headers, CancellationToken? cancellationToken = null) {
			ThrowIfDisposed();

			ArgumentNullException.ThrowIfNull(subscription, nameof(subscription));
			ArgumentNullException.ThrowIfNull(headers, nameof(headers));

			try {
				var existing = await SubscriptionRepository.GetHeadersAsync(subscription, CancellationToken);

				var toAdd = headers.Where(x => !existing.ContainsKey(x.Key)).ToArray();
				var toRemove = existing.Where(x => !headers.ContainsKey(x.Key)).ToArray();

				if (toAdd.Length == 0 && toRemove.Length == 0)
					return OperationResult.NotChanged;

				if (toAdd.Length > 0)
					await SubscriptionRepository.AddHeadersAsync(subscription, toAdd.ToDictionary(x => x.Key, x => x.Value), CancellationToken);
				if (toRemove.Length > 0)
					await SubscriptionRepository.RemoveHeadersAsync(subscription, toRemove.Select(x => x.Key).ToArray(), CancellationToken);

				return await UpdateAsync(subscription);
			} catch (Exception ex) {
				Logger.LogUnknownSubscriptionError(ex, subscription.SubscriptionId);
				return Fail(WebhookSubscriptionErrorCodes.UnknownError, "Unhandled error while setting the headers");
			}
		}

		/// <summary>
		/// Sets the URL where to send the webhook notifications 
		/// for a subscription.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to set the destination URL.
		/// </param>
		/// <param name="url">
		/// The URL address to set as destination of the notifications.
		/// </param>
		/// <param name="cancellationToken">
		/// A token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns an object that represents the result of the operation.
		/// </returns>
		public async Task<OperationResult> SetDestinationUrlAsync(TSubscription subscription, string url, CancellationToken cancellationToken = default) {
			try {
				var existing = SubscriptionRepository.GetDestinationUrlAsync(subscription, cancellationToken);
				if (existing != null && existing.Equals(url))
					return OperationResult.NotChanged;

				await SubscriptionRepository.SetDestinationUrlAsync(subscription, url, cancellationToken);

				return await UpdateAsync(subscription);
			} catch (Exception ex) {
				Logger.LogUnknownSubscriptionError(ex, subscription.SubscriptionId);
				return Fail(WebhookSubscriptionErrorCodes.UnknownError, "Unhandled error while setting the destination URL");
			}
		}

		/// <summary>
		/// Sets the secret that is used to sign the webhook notifications.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to set the secret.
		/// </param>
		/// <param name="secret">
		/// The secret word to set.
		/// </param>
		/// <param name="cancellationToken">
		/// A token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns an object that represents the result of the operation.
		/// </returns>
		public async Task<OperationResult> SetSecretAsync(TSubscription subscription, string? secret, CancellationToken? cancellationToken = null) {
			try {
				var existing = await SubscriptionRepository.GetSecretAsync(subscription, GetCancellationToken(cancellationToken));
				if (existing != null && existing.Equals(secret))
					return OperationResult.NotChanged;

				await SubscriptionRepository.SetSecretAsync(subscription, secret, GetCancellationToken(cancellationToken));

				return await UpdateAsync(subscription);
			} catch (Exception ex) {
				Logger.LogUnknownSubscriptionError(ex, subscription.SubscriptionId);
				return Fail(WebhookSubscriptionErrorCodes.UnknownError, "Unhandled error while setting the secret");
			}
		}

		/// <summary>
		/// Sets the properties of a subscription.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the subscription to set the properties.
		/// </param>
		/// <param name="properties">
		/// The list of properties to set.
		/// </param>
		/// <param name="cancellationToken">
		/// A token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns an object that represents the result of the operation.
		/// </returns>
		public async Task<OperationResult> SetPropertiesAsync(TSubscription subscription, IDictionary<string, object> properties, CancellationToken? cancellationToken = null) {
			try {
				var existing = await SubscriptionRepository.GetPropertiesAsync(subscription, GetCancellationToken(cancellationToken));

				var toAdd = properties.Where(x => !existing.ContainsKey(x.Key)).ToArray();
				var toRemove = existing.Where(x => !properties.ContainsKey(x.Key)).ToArray();

				if (toAdd.Length == 0 && toRemove.Length == 0)
					return OperationResult.NotChanged;

				if (toAdd.Length > 0)
					await SubscriptionRepository.AddPropertiesAsync(subscription, toAdd.ToDictionary(x => x.Key, x => x.Value), GetCancellationToken(cancellationToken));
				if (toRemove.Length > 0)
					await SubscriptionRepository.RemovePropertiesAsync(subscription, toRemove.Select(x => x.Key).ToArray(), GetCancellationToken(cancellationToken));

				return await UpdateAsync(subscription);
			} catch (Exception ex) {
				Logger.LogUnknownSubscriptionError(ex, subscription.SubscriptionId);
				return Fail(WebhookSubscriptionErrorCodes.UnknownError, "Unhandled error while setting the properties");
			}
		}
	}
}
