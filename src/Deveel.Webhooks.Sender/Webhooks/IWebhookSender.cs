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

using System;

namespace Deveel.Webhooks {
	/// <summary>
	/// A service that is able to send a webhook to a destination.
	/// </summary>
	/// <typeparam name="TWebhook">
	/// The type of the webhook to send.
	/// </typeparam>
	public interface IWebhookSender<TWebhook> where TWebhook : class {
		/// <summary>
		/// Sends the given webhook to the given receiver.
		/// </summary>
		/// <param name="receiver">
		/// An object that describes the destination of the webhook, including
		/// configurations to control the behavior of the sender.
		/// </param>
		/// <param name="webhook">
		/// The instance of the webhook to send to the destination.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a <see cref="WebhookDeliveryResult{TWebhook}"/> that summarizes
		/// the result of the delivery attempts to the destination.
		/// </returns>
		Task<WebhookDeliveryResult<TWebhook>> SendAsync(WebhookDestination receiver, TWebhook webhook, CancellationToken cancellationToken = default);
	}
}
