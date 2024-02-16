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

namespace Deveel.Webhooks {
	/// <summary>
	/// Extends the <see cref="ITenantWebhookNotifier{TWebhook}"/> with
	/// methods to notify a tenant of a single event.
	/// </summary>
	public static class TenantWebhookNotifierExtensions {
		/// <summary>
		/// Notifies the tenant of the given event.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of the webhook to be notified.
		/// </typeparam>
		/// <param name="notifier">
		/// The instance of the notifier to use to send the notification.
		/// </param>
		/// <param name="tenantId">
		/// The unique identifier of the tenant to notify.
		/// </param>
		/// <param name="eventInfo">
		/// The event that is being notified.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used to cancel the notification.
		/// </param>
		/// <returns>
		/// Returns the result of the notification process, that contains
		/// the notification of a single event.
		/// </returns>
		public static Task<WebhookNotificationResult<TWebhook>> NotifyAsync<TWebhook>(this ITenantWebhookNotifier<TWebhook> notifier, string tenantId, EventInfo eventInfo, CancellationToken cancellationToken = default)
			where TWebhook : class {
			return notifier.NotifyAsync(tenantId, new EventNotification(eventInfo), cancellationToken);
		}
	}
}
