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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	/// <summary>
	/// A service that resolves subscriptions to events, prepares
	/// and delivers webhooks to the subscribers.
	/// </summary>
	/// <typeparam name="TWebhook">The type of the webhook that is delivered</typeparam>
	/// <remarks>
	/// Implementations of this interface resolve subscriptions to events
	/// without any tenant scope explicitly set: despite of this condition,
	/// the service might still resolve subscriptions to events that are
	/// owned by tenants, if the discovery is performed by a service that
	/// this resolver invokes.
	/// </remarks>
	public interface IWebhookNotifier<TWebhook> where TWebhook : class {
		/// <summary>
		/// Notifies to the subscribers the occurrence of the
		/// given event.
		/// </summary>
		/// <param name="notification">
		/// The aggregate of the events to be notified to the subscribers.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the notification process.
		/// </param>
		/// <returns>
		/// Returns an object that describes the aggregated final result of 
		/// the notification process executed.
		/// </returns>
		Task<WebhookNotificationResult<TWebhook>> NotifyAsync(EventNotification notification, CancellationToken cancellationToken = default);
	}
}
