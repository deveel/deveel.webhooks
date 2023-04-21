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

using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	/// <summary>
	/// A default implementation of the <see cref="IWebhookFactory{TWebhook}"/>
	/// that creates a <see cref="Webhook"/> instance using the information
	/// from the subscription and the event.
	/// </summary>
	public sealed class DefaultWebhookFactory : IWebhookFactory<Webhook> {
		/// <summary>
		/// Creates a new webhook instance using the information
		/// from the subscription and the event.
		/// </summary>
		/// <param name="subscription">
		/// The subscription that is listening to the event
		/// </param>
		/// <param name="eventInfo">
		/// The event that is being delivered to the subscription
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation
		/// </param>
		/// <returns>
		/// Returns a task that resolves to the created webhook
		/// </returns>
		public Task<Webhook> CreateAsync(IWebhookSubscription subscription, EventInfo eventInfo, CancellationToken cancellationToken) {
			var webhook = new Webhook {
				Id = eventInfo.Id,
				EventType = eventInfo.EventType,
				SubscriptionId = subscription.SubscriptionId,
				TimeStamp = eventInfo.TimeStamp,
				Data = eventInfo.Data,
			};

			return Task.FromResult(webhook);
		}
	}
}
