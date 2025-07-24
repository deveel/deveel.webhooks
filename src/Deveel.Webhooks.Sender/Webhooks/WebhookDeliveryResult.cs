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

using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace Deveel.Webhooks {
	/// <summary>
	/// An object that presents a summary of the delivery of a webhook
	/// to a destination point.
	/// </summary>
	/// <typeparam name="TWebhook">
	/// The type of the webhook that was delivered.
	/// </typeparam>
	/// <remarks>
	/// <para>
	/// This result is the composition of a number of <see cref="WebhookDeliveryAttempt"/>,
	/// that are done by a <see cref="IWebhookSender{TWebhook}"/> to deliver the given
	/// webhook to a destination: the overall status of the delivery is the compound
	/// of the statuses of the individual attempts.
	/// </para>
	/// <para>
	/// A delivery is considered successful if at least one of the attempts were successful.
	/// </para>
	/// </remarks>
    public sealed class WebhookDeliveryResult<TWebhook> where TWebhook : class {
		private readonly ConcurrentBag<WebhookDeliveryAttempt> attempts = new ConcurrentBag<WebhookDeliveryAttempt>();

		/// <summary>
		/// Constructs a new delivery result for the given webhook to the given destination.
		/// </summary>
		/// <param name="operationId">
		/// The unique identifier of the operation that delivered the webhook.
		/// </param>
		/// <param name="destination">
		/// The destination of the webhook.
		/// </param>
		/// <param name="webhook">
		/// The webhook that was delivered.
		/// </param>
		public WebhookDeliveryResult(string operationId, WebhookDestination destination, TWebhook webhook) {
			if (string.IsNullOrWhiteSpace(operationId)) 
				throw new ArgumentException($"'{nameof(operationId)}' cannot be null or whitespace.", nameof(operationId));

			Destination = destination ?? throw new ArgumentNullException(nameof(destination));
			Webhook = webhook ?? throw new ArgumentNullException(nameof(webhook));
			OperationId = operationId;
		}

		/// <summary>
		/// Gets the unique identifier of the operation that delivered the webhook.
		/// </summary>
		public string OperationId { get; }

		/// <summary>
		/// Gets a read-only list of the delivery attempts that were made
		/// </summary>
		/// <seealso cref="WebhookDeliveryAttempt"/>
		public IReadOnlyList<WebhookDeliveryAttempt> Attempts 
			=> attempts.OrderBy(x => x.Number).ToList().AsReadOnly();

		/// <summary>
		/// Gets the object that describes the destination of the webhook.
		/// </summary>
		public WebhookDestination Destination { get; }

		/// <summary>
		/// Gets the instance of the webhook that was delivered.
		/// </summary>
		public TWebhook Webhook { get; }

		/// <summary>
		/// Gets the last attempt made to deliver the webhook,
		/// or <c>null</c> if no attempt was made yet.
		/// </summary>
		/// <seealso cref="HasAttempted"/>
		public WebhookDeliveryAttempt? LastAttempt => 
			Attempts.OrderByDescending(x => x.Number).FirstOrDefault();

		/// <summary>
		/// Gets a flag indicating if at least one attempt was made to deliver
		/// </summary>
		public bool HasAttempted => attempts.Count > 0;

		/// <summary>
		/// Gets a boolean value indicating if at least one delivery attempt was successful.
		/// </summary>
		/// <seealso cref="WebhookDeliveryAttempt.Succeeded"/>
		public bool Successful => Attempts.Any(x => x.Succeeded);

		/// <summary>
		/// Gets the number of attempts made to deliver the webhook.
		/// </summary>
		public int AttemptCount => attempts.Count;

		/// <summary>
		/// Starts a new delivery attempt for the webhook.
		/// </summary>
		/// <remarks>
		/// Once this method is called, a new <see cref="WebhookDeliveryAttempt"/> is created
		/// and registered for this delivery result.
		/// </remarks>
		/// <returns>
		/// Returns a new instance of <see cref="WebhookDeliveryAttempt"/> that can be used
		/// to track the delivery attempt status.
		/// </returns>
		public WebhookDeliveryAttempt StartAttempt() {
			var attempt = WebhookDeliveryAttempt.Start(attempts.Count + 1);
			attempts.Add(attempt);

			return attempt;
		}
	}
}
