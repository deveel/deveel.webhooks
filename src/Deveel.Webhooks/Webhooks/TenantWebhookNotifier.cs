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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	/// <summary>
	/// The default implementation of the webhook notifier
	/// that addresses specific tenants
	/// </summary>
	public class TenantWebhookNotifier<TWebhook> : WebhookNotifierBase<TWebhook>, ITenantWebhookNotifier<TWebhook> where TWebhook : class {
		private readonly ITenantWebhookSubscriptionResolver? subscriptionResolver;

		/// <summary>
		/// Constructs the notifier with the given services.
		/// </summary>
		/// <param name="options">
		/// The configuration options of the notifier.
		/// </param>
		/// <param name="sender">
		/// The service used to send the webhook.
		/// </param>
		/// <param name="subscriptionResolver">
		/// A service used to resolve the subscriptions owned by a
		/// tanant that will be notified
		/// </param>
		/// <param name="webhookFactory">
		/// A service used to create the webhook to send.
		/// </param>
		/// <param name="filterEvaluators">
		/// A collection of services used to filter the webhooks to
		/// be delivered
		/// </param>
		/// <param name="deliveryResultLogger">
		/// A service used to log the delivery result of the webhook.
		/// </param>
		/// <param name="logger">
		/// A logger used to log the activity of the notifier.
		/// </param>
		public TenantWebhookNotifier(
			IOptions<WebhookNotificationOptions<TWebhook>> options,
			IWebhookSender<TWebhook> sender,
			IWebhookFactory<TWebhook> webhookFactory,
			ITenantWebhookSubscriptionResolver<TWebhook>? subscriptionResolver = null,
			IEnumerable<IWebhookFilterEvaluator<TWebhook>>? filterEvaluators = null,
			IWebhookDeliveryResultLogger<TWebhook>? deliveryResultLogger = null,
			ILogger<TenantWebhookNotifier<TWebhook>>? logger = null) 
			: base(options.Value, sender, webhookFactory, filterEvaluators, deliveryResultLogger, logger) {
			this.subscriptionResolver = subscriptionResolver;
		}

		/// <summary>
		/// Resolves the subscriptions that should be notified for the given event.
		/// </summary>
		/// <param name="tenantId">
		/// The identifier of the tenant for which the event was raised.
		/// </param>
		/// <param name="eventInfo">
		/// The information about the event that was raised.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a list of subscriptions that should be notified for the given event.
		/// </returns>
		protected virtual async Task<IList<IWebhookSubscription>> ResolveSubscriptionsAsync(string tenantId, EventInfo eventInfo, CancellationToken cancellationToken) {
			if (subscriptionResolver == null)
				return new List<IWebhookSubscription>();

			try {
				return await subscriptionResolver.ResolveSubscriptionsAsync(tenantId, eventInfo.EventType, true, cancellationToken);
			} catch(WebhookException) {
				throw;
			} catch (Exception ex) {
				throw new WebhookException("An error occurred while trying to resolve the subscriptions", ex);
			}
		}

		/// <inheritdoc/>
		public virtual async Task<WebhookNotificationResult<TWebhook>> NotifyAsync(string tenantId, EventInfo eventInfo, CancellationToken cancellationToken) {
			IEnumerable<IWebhookSubscription> subscriptions;

			try {
				subscriptions = await ResolveSubscriptionsAsync(tenantId, eventInfo, cancellationToken);
			} catch (WebhookException ex) {
				Logger.LogError(ex, "Error while resolving the subscriptions to event {EventType} for tenant '{TenantId}'", 
					eventInfo.EventType, tenantId);
				throw;
			}

			return await NotifySubscriptionsAsync(eventInfo, subscriptions, cancellationToken);
		}
	}
}
