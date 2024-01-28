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
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	/// <summary>
	/// Defines a service that is able to log the result of 
	/// a delivery of a webhook
	/// </summary>
	/// <typeparam name="TWebhook">
	/// The type of the webhook that is delivered
	/// </typeparam>
	public interface IWebhookDeliveryResultLogger<TWebhook> where TWebhook : class {
		/// <summary>
		/// Logs the result of a delivery of a webhook
		/// </summary>
		/// <param name="eventInfo">
		/// The information about the event that triggered the notification.
		/// </param>
		/// <param name="subscription">
		/// The subscription that was used to deliver the webhook
		/// </param>
		/// <param name="result">
		/// The result of the delivery of the webhook
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation
		/// </param>
		/// <returns>
		/// Returns a task that when completed will log the result of the delivery
		/// </returns>
		Task LogResultAsync(EventInfo eventInfo, IWebhookSubscription subscription, WebhookDeliveryResult<TWebhook> result, CancellationToken cancellationToken = default);
	}
}
