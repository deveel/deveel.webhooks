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
	/// A service that sends webhooks to the final receivers.
	/// </summary>
	public interface IWebhookSender<TWebhook> where TWebhook : class {
		/// <summary>
		/// Sends a single webhook to the given recipient.
		/// </summary>
		/// <param name="webhook">The webhook to be delivered.</param>
		/// <param name="cancellationToken"></param>
		/// <returns>
		/// Returns an object that describes the final result of
		/// the delivery operation.
		/// </returns>
		Task<WebhookDeliveryResult> SendAsync(TWebhook webhook, CancellationToken cancellationToken);
	}
}
