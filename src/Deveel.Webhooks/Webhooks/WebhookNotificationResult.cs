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

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace Deveel.Webhooks {
	/// <summary>
	/// Repsents an aggregated result of the notification of an event.
	/// </summary>
	/// <typeparam name="TWebhook">
	/// The type of the webhook notified.
	/// </typeparam>
	public sealed class WebhookNotificationResult<TWebhook> : IEnumerable<KeyValuePair<string, IList<WebhookDeliveryResult<TWebhook>>>>
		where TWebhook : class {
		private readonly ConcurrentDictionary<string, IList<WebhookDeliveryResult<TWebhook>>> deliveryResults;

		/// <summary>
		/// Constructs a notification result for the given event.
		/// </summary>
		/// <param name="notification">
		/// The aggregate of the events to be notified to the subscribers.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the given <paramref name="notification"/> is <c>null</c>.
		/// </exception>
		public WebhookNotificationResult(EventNotification notification) {
			ArgumentNullException.ThrowIfNull(notification, nameof(notification));

			deliveryResults = new ConcurrentDictionary<string, IList<WebhookDeliveryResult<TWebhook>>>();
			Notification = notification;
		}

		/// <summary>
		/// Gets the information of the event that was notified.
		/// </summary>
		public EventNotification Notification { get; }

		/// <summary>
		/// Adds a delivery result for the given subscription.
		/// </summary>
		/// <param name="subscriptionId">
		/// The identifier of the subscription that was notified.
		/// </param>
		/// <param name="result">
		/// The result of the delivery of the notification.
		/// </param>
		public void AddDelivery(string subscriptionId, WebhookDeliveryResult<TWebhook> result) {
			lock (this) {
				deliveryResults.AddOrUpdate(subscriptionId, new List<WebhookDeliveryResult<TWebhook>> { result }, (s, list) => {
					list.Add(result);
					return list;
				});
			}
		}

		/// <inheritdoc/>
		public IEnumerator<KeyValuePair<string, IList<WebhookDeliveryResult<TWebhook>>>> GetEnumerator()
			=> deliveryResults.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <summary>
		/// Gets a value indicating if the notification has any 
		/// successful deliveries
		/// </summary>
		public bool HasSuccessful => Successful?.Any() ?? false;

		/// <summary>
		/// Gets the list of successful deliveries.
		/// </summary>
		public IEnumerable<KeyValuePair<string, IList<WebhookDeliveryResult<TWebhook>>>> Successful
			=> deliveryResults.Where(x => x.Value.Any(y => y.Successful));

		/// <summary>
		/// Gets a value indicating if the notification has any
		/// deliveries that failed.
		/// </summary>
		public bool HasFailed => Failed?.Any() ?? false;

		/// <summary>
		/// Gets the list of deliveries that failed.
		/// </summary>
		public IEnumerable<KeyValuePair<string, IList<WebhookDeliveryResult<TWebhook>>>> Failed
			=> deliveryResults.Where(x => x.Value.All(y => !y.Successful));

		/// <summary>
		/// Gets a value indicating if the notification has any 
		/// deliveries at all
		/// </summary>
		public bool IsEmpty => deliveryResults.Count == 0;

		/// <summary>
		/// Gets the list of delivery results for the given subscription.
		/// </summary>
		/// <param name="subscriptionId">
		/// The identifier of the subscription.
		/// </param>
		/// <returns>
		/// Returns a list of <see cref="WebhookDeliveryResult{TWebhook}"/> for the given
		/// subscription, if any.
		/// </returns>
		public IReadOnlyList<WebhookDeliveryResult<TWebhook>>? this[string subscriptionId] {
			get {
				if (!deliveryResults.TryGetValue(subscriptionId, out var results))
					return null;

				return new ReadOnlyCollection<WebhookDeliveryResult<TWebhook>>(results);
			}
		}
	}
}