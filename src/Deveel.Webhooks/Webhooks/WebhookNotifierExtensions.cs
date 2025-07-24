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

namespace Deveel.Webhooks {
	/// <summary>
	/// Extensions for <see cref="IWebhookNotifier{TWebhook}"/> to notify
	/// a single event to the subscribers.
	/// </summary>
	public static class WebhookNotifierExtensions {
		/// <summary>
		/// Notifies the given event to the subscribers.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of the webhook to be notified.
		/// </typeparam>
		/// <param name="notifier">
		/// The instance of the notifier service used to deliver
		/// the notification.
		/// </param>
		/// <param name="eventInfo">
		/// The event to be notified to the subscribers.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the notification.
		/// </param>
		/// <returns>
		/// Returns the result of the notification that contains
		/// a single event.
		/// </returns>
		public static Task<WebhookNotificationResult<TWebhook>> NotifyAsync<TWebhook>(this IWebhookNotifier<TWebhook> notifier, EventInfo eventInfo, CancellationToken cancellationToken = default)
			where TWebhook : class {
			return notifier.NotifyAsync(new EventNotification(eventInfo), cancellationToken);
		}
	}
}
