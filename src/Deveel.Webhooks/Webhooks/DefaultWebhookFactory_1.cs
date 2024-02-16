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

using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	/// <summary>
	/// A default implementation of the <see cref="IWebhookFactory{TWebhook}"/>
	/// that creates a <see cref="Webhook"/> instance using the information
	/// from the subscription and the event.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The sender and notifier services are not bound to a specific
	/// type of webhook, and this factory is used to create the default type
	/// of webhook provided for convenience.
	/// </para>
	/// <para>
	/// It is possible to create a custom webhook type by implementing
	/// this factory and overriding the <see cref="CreateNotificationData(IWebhookSubscription, EventNotification)"/>
	/// </para>
	/// <para>
	/// By default the <see cref="Webhook"/> class is configured with attributes from
	/// the <c>System.Text.Json</c> library and the <c>System.Xml</c> namespace, so that 
	/// the serialization process recognizes them and uses them to serialize the data:
	/// if you need to use other serializers you might experience inconsistent results.
	/// </para>
	/// </remarks>
	/// <seealso cref="Webhook"/>
	public class DefaultWebhookFactory<TWebhook> : IWebhookFactory<TWebhook> where TWebhook : Webhook, new() {
		/// <summary>
		/// Constructs the factory with the options to use when creating
		/// webhooks from a notification for a subscription.
		/// </summary>
		/// <param name="options"></param>
		public DefaultWebhookFactory(IOptions<WebhookFactoryOptions<TWebhook>>? options = null) {
			Options = options?.Value ?? new WebhookFactoryOptions<TWebhook>();
		}

		protected WebhookFactoryOptions<TWebhook> Options { get; }

		/// <summary>
		/// When overridden, creates the data object that is carried
		/// by the webhook to the receiver.
		/// </summary>
		/// <param name="subscription">
		/// The subscription that is listening to the event
		/// </param>
		/// <param name="notification">
		/// The aggregate of the events that are being delivered to the subscription.
		/// </param>
		/// <remarks>
		/// The default implementation of this method returns an array of
		/// objects, each one created by the <see cref="CreateEventData(IWebhookSubscription, EventInfo)"/>,
		/// when the notification contains multiple events, or the single object
		/// if the notification contains a single event.
		/// </remarks>
		/// <returns>
		/// Returns a data object that is carried by the webhook
		/// through the <see cref="Webhook.Data"/> property.
		/// </returns>
		protected virtual object? CreateNotificationData(IWebhookSubscription subscription, EventNotification notification) {
			if (Options.CreateStrategy == WebhookCreateStrategy.OnePerNotification) {
				if (notification.Events.Count == 1)
					return CreateEventData(subscription, notification.Events[0]);

				return notification.Select(e => CreateEventData(subscription, e)).ToArray();
			}

			if (notification.Events.Count == 1)
				return CreateEventData(subscription, notification.Events[0]);

			throw new WebhookException("The strategy 'OnePerEvent' requires a single event in the notification");
		}

		/// <summary>
		/// When overridden, creates the data object that is carried by
		/// a webhook to the receiver.
		/// </summary>
		/// <param name="subscription"></param>
		/// <param name="eventInfo"></param>
		/// <remarks>
		/// The default implementation of this method returns the <see cref="EventInfo.Data"/>
		/// object of the given event.
		/// </remarks>
		/// <returns></returns>
		protected virtual object? CreateEventData(IWebhookSubscription subscription, EventInfo eventInfo) {
			return eventInfo.Data;
		}

		/// <summary>
		/// Creates a new webhook instance using the information
		/// from the subscription and the event.
		/// </summary>
		/// <param name="subscription">
		/// The subscription that is listening to the event
		/// </param>
		/// <param name="notification">
		/// The event that is being delivered to the subscription
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation
		/// </param>
		/// <returns>
		/// Returns a task that resolves to the created webhook
		/// </returns>
		public virtual Task<IList<TWebhook>> CreateAsync(IWebhookSubscription subscription, EventNotification notification, CancellationToken cancellationToken) {
			IList<TWebhook> result;

			if (Options.CreateStrategy == WebhookCreateStrategy.OnePerEvent) {
				result = notification.Events.Select(e => new TWebhook {
					Id = e.Id,
					EventType = e.EventType,
					SubscriptionId = subscription.SubscriptionId,
					Name = subscription.Name,
					TimeStamp = e.TimeStamp,
					Data = CreateEventData(subscription, e),
				}).ToList();
			} else {
				result = new List<TWebhook> { new TWebhook {
					Id = notification.NotificationId,
					EventType = notification.EventType,
					SubscriptionId = subscription.SubscriptionId,
					Name = subscription.Name,
					TimeStamp = notification.TimeStamp,
					Data = CreateNotificationData(subscription, notification),
				}
				};
			}

			return Task.FromResult(result);
		}
	}
}