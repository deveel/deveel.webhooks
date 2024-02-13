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
	/// Defines a factory that can create <typeparamref name="TWebhook"/> 
	/// instances given the subscription and the events notification.
	/// </summary>
	/// <typeparam name="TWebhook">
	/// The type of the webhook instance to create.
	/// </typeparam>
	public interface IWebhookFactory<TWebhook> where TWebhook : class {
		/// <summary>
		/// Creates a new instance of the webhook given the subscription
		/// </summary>
		/// <param name="subscription">
		/// The subscription that is requesting the webhook.
		/// </param>
		/// <param name="notification">
		/// The notification that is being delivered to the receiver.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns an instance of the webhook that will be delivered to
		/// the receiver that is subscribed to the event.
		/// </returns>
		Task<IList<TWebhook>> CreateAsync(IWebhookSubscription subscription, EventNotification notification, CancellationToken cancellationToken = default);
	}
}
