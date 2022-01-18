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
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides the aggregated information of a webhook
	/// delivery process, through all the attempts.
	/// </summary>
	public sealed class WebhookDeliveryResult {
		private readonly List<WebhookDeliveryAttempt> _attempts;
		private readonly bool failed;

		private WebhookDeliveryResult(IWebhook webhook, bool failed) {
			Webhook = webhook ?? throw new ArgumentNullException(nameof(webhook));
			this.failed = failed;
			_attempts = new List<WebhookDeliveryAttempt>();
		}

		/// <summary>
		/// Construct a descriptor that is used to handle updates
		/// sequentially by the sender.
		/// </summary>
		/// <param name="webhook">The webhook that is to be delivered.</param>
		public WebhookDeliveryResult(IWebhook webhook)
			: this(webhook, false) {
		}

		public IWebhook Webhook { get; }

		public IEnumerable<WebhookDeliveryAttempt> Attempts => _attempts.AsReadOnly();

		/// <summary>
		/// Creates a delivery result that failed before any attempt was done.
		/// </summary>
		/// <param name="webhook">The webhook that was not even attempted to be delivered.</param>
		/// <returns>
		/// Returns a <see cref="WebhookDeliveryResult"/> that indicates a failure
		/// before any attempt.
		/// </returns>
		public static WebhookDeliveryResult Fail(IWebhook webhook) => new WebhookDeliveryResult(webhook, true);

		/// <summary>
		/// Gets the last attempt of delivery that was registered
		/// </summary>
		public WebhookDeliveryAttempt LastAttempt => Attempts
			.Where(x => x.EndedAt != null)
			.OrderByDescending(x => x.EndedAt.Value).FirstOrDefault();

		/// <summary>
		/// Gets a value indicating whether any attempt to deliver the webhook
		/// was performed or registered.
		/// </summary>
		public bool HasAttempted => _attempts.Count > 0;

		/// <summary>
		/// Gets a value indicating if the overall delivery was
		/// successfull through at least one of the attempts.
		/// </summary>
		public bool Successful => !failed && Attempts.All(x => !x.Failed);

		/// <summary>
		/// Registers a new attempt of delivery of the webhook.
		/// </summary>
		/// <param name="attempt"></param>
		public void AddAttempt(WebhookDeliveryAttempt attempt) {
			lock (this) {
				_attempts.Add(attempt);
			}
		}

		public WebhookDeliveryAttempt StartAttempt() {
			lock (this) {
				var attempt = new WebhookDeliveryAttempt();
				_attempts.Add(attempt);

				return attempt;
			}
		}
	}
}
