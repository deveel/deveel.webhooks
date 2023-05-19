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

namespace Deveel.Webhooks {
	/// <summary>
	/// Describes the result of a webhook delivery operation
	/// </summary>
	public interface IWebhookDeliveryResult {
		/// <summary>
		/// Gets the identifier of the operation that
		/// attempted to deliver the webhook
		/// </summary>
		string OperationId { get; }

		/// <summary>
		/// Gets the information about the event that
		/// triggered the notification
		/// </summary>
		IEventInfo EventInfo { get; }

		/// <summary>
		/// Gets the webhook that was notified
		/// </summary>
		IWebhook Webhook { get; }

		/// <summary>
		/// Gets the information about the receiver
		/// of the webhook
		/// </summary>
		IWebhookReceiver Receiver { get;}

		/// <summary>
		/// Gets a list of all the delivery attempts made
		/// during the notification of the webhook
		/// </summary>
		IEnumerable<IWebhookDeliveryAttempt> DeliveryAttempts { get; }
	}
}
