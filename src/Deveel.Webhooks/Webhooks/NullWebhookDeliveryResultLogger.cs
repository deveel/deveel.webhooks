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

namespace Deveel.Webhooks {
	/// <summary>
	/// A default implementation of <see cref="IWebhookDeliveryResultLogger{TWebhook}"/> that
	/// performs no logging.
	/// </summary>
	/// <typeparam name="TWebhook">
	/// The type of webhook that is being delivered.
	/// </typeparam>
	public sealed class NullWebhookDeliveryResultLogger<TWebhook> : IWebhookDeliveryResultLogger<TWebhook>
		where TWebhook : class {
		private NullWebhookDeliveryResultLogger() {
		}

		/// <summary>
		/// Gets the singleton instance of the logger.
		/// </summary>
		public static readonly NullWebhookDeliveryResultLogger<TWebhook> Instance = new NullWebhookDeliveryResultLogger<TWebhook>();

		/// <inheritdoc/>
		public Task LogResultAsync(EventNotification notification, IWebhookSubscription subscription, WebhookDeliveryResult<TWebhook> result, CancellationToken cancellationToken) {
			return Task.CompletedTask;
		}
	}
}
