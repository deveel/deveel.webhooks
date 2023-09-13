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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	/// <summary>
	/// A notification service that is scoped to a specific webhook type.
	/// </summary>
	/// <typeparam name="TWebhook">
	/// The type of the webhook that is scoped for the notifier.
	/// </typeparam>
	public class WebhookNotifier<TWebhook> : WebhookNotifierBase<TWebhook>, IWebhookNotifier<TWebhook> where TWebhook : class {
		private readonly IWebhookSubscriptionResolver<TWebhook>? subscriptionResolver;

		/// <summary>
		/// Constructs the notifier with the given sender and factory.
		/// </summary>
		/// <param name="options">
		/// The configuration options of the notifier.
		/// </param>
		/// <param name="sender">
		/// The service instance that will be used to send the notifications.
		/// </param>
		/// <param name="webhookFactory">
		/// A factory of webhooks that will be notified
		/// </param>
		/// <param name="subscriptionResolver">
		/// A service used to resolve the subscriptions to a given event
		/// </param>
		/// <param name="filterEvaluators">
		/// A list of all the evaluators registered in the application context,
		/// and that will be used to filter the webhooks to be notified.
		/// </param>
		/// <param name="deliveryResultLogger">
		/// An optional service used to log the delivery result of the webhook.
		/// </param>
		/// <param name="logger">
		/// A logger instance used to log the activity of the notifier.
		/// </param>
		public WebhookNotifier(
			IOptions<WebhookNotificationOptions<TWebhook>> options,
			IWebhookSender<TWebhook> sender, 
			IWebhookFactory<TWebhook> webhookFactory,
			IWebhookSubscriptionResolver<TWebhook>? subscriptionResolver = null,
			IEnumerable<IWebhookFilterEvaluator<TWebhook>>? filterEvaluators = null, 
			IWebhookDeliveryResultLogger<TWebhook>? deliveryResultLogger = null, 
			ILogger<WebhookNotifier<TWebhook>>? logger = null) 
			: base(options.Value, sender, webhookFactory, filterEvaluators, deliveryResultLogger, logger) {
			this.subscriptionResolver = subscriptionResolver;
		}

		/// <inheritdoc/>
		protected virtual async Task<IEnumerable<IWebhookSubscription>> ResolveSubscriptionsAsync(EventInfo eventInfo, CancellationToken cancellationToken) {
			if (subscriptionResolver == null)
				return new List<IWebhookSubscription>();

			try {
				return await subscriptionResolver.ResolveSubscriptionsAsync(eventInfo, true, cancellationToken);
			} catch (WebhookException) {
				throw;
			} catch (Exception ex) {
				throw new WebhookException("An error occurred while trying to resolve the subscriptions", ex);
			}

		}

		/// <inheritdoc/>
		public async Task<WebhookNotificationResult<TWebhook>> NotifyAsync(EventInfo eventInfo, CancellationToken cancellationToken) {
			var subscriptions = await ResolveSubscriptionsAsync(eventInfo, cancellationToken);

			return await NotifySubscriptionsAsync(eventInfo, subscriptions, cancellationToken);

		}
	}
}
