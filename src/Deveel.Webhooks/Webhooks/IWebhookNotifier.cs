// Copyright 2022 Deveel
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
	/// A service that resolves subscriptions to events, prepares
	/// and delivers webhooks to the subscribers.
	/// </summary>
	public interface IWebhookNotifier {
		/// <summary>
		/// Notifies to the subscribers the occurrence of the
		/// given event.
		/// </summary>
		/// <param name="tenantId">The scope of the tenant holding the subscriptions
		/// to the given event.</param>
		/// <param name="eventInfo">The ifnormation of the event that occurred.</param>
		/// <param name="cancellationToken"></param>
		/// <returns>
		/// Returns an object that describes the aggregated final result of 
		/// the notification process executed.
		/// </returns>
		Task<WebhookNotificationResult> NotifyAsync(string tenantId, EventInfo eventInfo, CancellationToken cancellationToken);
	}
}
