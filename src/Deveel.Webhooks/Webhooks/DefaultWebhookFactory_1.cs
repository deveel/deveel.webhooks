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
	/// <remarks>
	/// <para>
	/// The sender and notifier services are not bound to a specific
	/// type of webhook, and this factory is used to create the default type
	/// of webhook provided for convenience.
	/// </para>
	/// <para>
	/// It is possible to create a custom webhook type by implementing
	/// this factory and overriding the <see cref="CreateData(IWebhookSubscription, EventInfo)"/>
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
		/// When overridden, creates the data object that is carried
		/// by the webhook to the receiver.
		/// </summary>
		/// <param name="subscription">
		/// The subscription that is listening to the event
		/// </param>
		/// <param name="eventInfo">
		/// The event that is being delivered to the subscription
		/// </param>
		/// <returns>
		/// Returns a data object that is carried by the webhook
		/// through the <see cref="Webhook.Data"/> property.
		/// </returns>
		protected virtual object CreateData(IWebhookSubscription subscription, EventInfo eventInfo) {
			return eventInfo.Data;
		}

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
		public Task<TWebhook> CreateAsync(IWebhookSubscription subscription, EventInfo eventInfo, CancellationToken cancellationToken) {
			var webhook = new TWebhook {
				Id = eventInfo.Id,
				EventType = eventInfo.EventType,
				SubscriptionId = subscription.SubscriptionId,
				Name = subscription.Name,
				TimeStamp = eventInfo.TimeStamp,
				Data = eventInfo.Data,
			};

			return Task.FromResult(webhook);
		}
	}
}
