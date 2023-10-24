﻿// Copyright 2022-2023 Deveel
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
	public class WebhookSubscriptionManager<TSubscription> : EntityManager<TSubscription>
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
			IWebhookSubscriptionRepository<TSubscription> subscriptionStore,
			IWebhookSubscriptionValidator<TSubscription>? validator = null,
			IServiceProvider? services = null,
			ILogger<WebhookSubscriptionManager<TSubscription>>? logger = null)
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
		protected virtual IWebhookSubscriptionRepository<TSubscription> SubscriptionRepository {
			get {
				ThrowIfDisposed();
				return (IWebhookSubscriptionRepository<TSubscription>) base.Repository;
			}
		}

		// TODO: fix this in Deveel Repository, to configure if to 
		//       compare the two entities and disable the comparison by default...
		/// <inheritdoc/>
		protected override bool AreEqual(TSubscription existing, TSubscription other) => false;

		public virtual Task<WebhookSubscriptionStatus> GetStatusAsync(TSubscription subscription, CancellationToken? cancellationToken = null) {
			try {
				return SubscriptionRepository.GetStatusAsync(subscription, GetCancellationToken(cancellationToken));
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while trying to get the status of subscription {SubscriptionId}", subscription.SubscriptionId);
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
				Logger.LogError(ex, "Error while trying to get the subscriptions for event type {EventType}", eventType);
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
        /// <returns>
        /// Returns <c>true</c> if the status was changed, or <c>false</c>
        /// </returns>
        /// <exception cref="WebhookException">
        /// Thrown if the status of the subscription could not be changed because of
        /// an unhanded exception.
        /// </exception>
        public async Task<OperationResult> SetStatusAsync(TSubscription subscription, WebhookSubscriptionStatus status) {
            try {
				var currentStatus = await SubscriptionRepository.GetStatusAsync(subscription, CancellationToken);

                if (currentStatus == status) {
                    Logger.LogTrace("The subscription {SubscriptionId} is already {Status}", subscription.SubscriptionId, status);
					return OperationResult.NotModified;
                }

                await SubscriptionRepository.SetStatusAsync(subscription, status, CancellationToken);

                Logger.LogInformation("The status of subscription {SubscriptionId} was changed to {Status}", subscription.SubscriptionId, status);

				return await UpdateAsync(subscription);
            } catch (WebhookException ex) {
				Logger.LogError(ex, "Error while trying to change the status of subscription {SubscriptionId}", subscription.SubscriptionId);
				return Fail("WEBHOOK_UNKNOWN_ERROR", "Unhandled error while changing the status");
            } catch (Exception ex) {
                Logger.LogError(ex, "Error while trying to change the state of subscription {SubscriptionId}", subscription.SubscriptionId);
				return Fail("WEBHOOK_UNKNOWN_ERROR", "Unhandled error while changing the status");
            }
        }

        /// <summary>
        /// Disables a webhook subscription by setting its status to <see cref="WebhookSubscriptionStatus.Suspended"/>.
        /// </summary>
        /// <param name="subscription">
        /// The instance of the webhook subscription to disable.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> if the subscription was disabled, or <c>false</c>
        /// if the subscription was already disabled.
        /// </returns>
        /// <seealso cref="SetStatusAsync(TSubscription, WebhookSubscriptionStatus)"/>
        public virtual Task<OperationResult> DisableAsync(TSubscription subscription)
			=> SetStatusAsync(subscription, WebhookSubscriptionStatus.Suspended);

		/// <summary>
		/// Enables a webhook subscription by setting its status to <see cref="WebhookSubscriptionStatus.Active"/>.
		/// </summary>
		/// <param name="subscription">
		/// The instance of the webhook subscription to enable.
		/// </param>
		/// <returns>
		/// Returns <c>true</c> if the subscription was enabled, or <c>false</c>
		/// if the subscription was already enabled.
		/// </returns>
		public virtual Task<OperationResult> EnableAsync(TSubscription subscription) 
			=> SetStatusAsync(subscription, WebhookSubscriptionStatus.Active);

		/// <inheritdoc/>
		public virtual async Task<long> CountAllAsync() {
			try {
				return await base.CountAsync(QueryFilter.Empty);
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while trying to count all webhook subscriptions");
				throw new WebhookException("Could not count the subscriptions", ex);
			}
		}
	}
}
