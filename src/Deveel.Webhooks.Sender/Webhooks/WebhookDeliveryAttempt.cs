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

using System.Net;

namespace Deveel.Webhooks {
	/// <summary>
	/// Describes a single attempt to deliver a webhook to a receiver.
	/// </summary>
	/// <remarks>
	/// In the overall delivery process, a webhook is delivered to a receiver
	/// through one or more attempts, and each attempt is described by an instance
	/// of this class.
	/// </remarks>
	/// <seealso cref="WebhookDeliveryResult{TWebhook}"/>
	public sealed class WebhookDeliveryAttempt {
		private WebhookDeliveryAttempt(int number, DateTimeOffset startedAt) {
			Number = number;
			StartedAt = startedAt;
		}

		/// <summary>
		/// Gets the ordinal number of the attempt in the context
		/// of a <see cref="WebhookDeliveryResult{TWebhook}"/>.
		/// </summary>
		public int Number { get; }

		/// <summary>
		/// Gets a HTTP status code returned by the receiver, if any.
		/// </summary>
		public int? ResponseCode { get; private set; }

		/// <summary>
		/// Gets a message returned by the receiver, if any.
		/// </summary>
		public string? ResponseMessage { get; private set; }

		/// <summary>
		/// Gets the time when the attempt started.
		/// </summary>
		public DateTimeOffset StartedAt { get; }

		/// <summary>
		/// Gets the time when the attempt completed, if any.
		/// </summary>
		public DateTimeOffset? CompletedAt { get; private set; }

		/// <summary>
		/// Gets a value indicating whether the attempt has completed.
		/// </summary>
		public bool HasCompleted => CompletedAt != null;

		/// <summary>
		/// Gets a value indicating whether the attempt has received any 
		/// response from the receiver.
		/// </summary>
		public bool HasResponse => ResponseCode != null;

		/// <summary>
		/// Gets a value indicating whether the attempt has failed.
		/// </summary>
		public bool Failed => (HasCompleted && ResponseCode == null) || (ResponseCode >= 400);

		/// <summary>
		/// Gets a value indicating whether the attempt has succeeded.
		/// </summary>
		public bool Succeeded => HasCompleted && ResponseCode != null && ResponseCode < 400;

		/// <summary>
		/// Gets a time-span elapsed between the start and the completion,
		/// or <c>null</c> if the attempt has not completed yet.
		/// </summary>
		public TimeSpan? Elapsed => CompletedAt?.Subtract(StartedAt);

		/// <summary>
		/// Creates a new attempt with the given ordinal number.
		/// </summary>
		/// <param name="number">
		/// The ordinal number of the attempt within the scope of delivery result.
		/// </param>
		/// <remarks>
		/// This constructor sets the <see cref="StartedAt"/> value to the current
		/// UTC time.
		/// </remarks>
		/// <returns>
		/// Returns a new instance of <see cref="WebhookDeliveryAttempt"/>.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when the given <paramref name="number"/> is less than 1.
		/// </exception>
		public static WebhookDeliveryAttempt Start(int number) {
			if (number < 1)
				throw new ArgumentOutOfRangeException(nameof(number), "The number of the attempt must be greater than zero.");

			return new WebhookDeliveryAttempt(number, DateTimeOffset.UtcNow);
		}

		/// <summary>
		/// Signals that the attempt has completed with the given response code
		/// </summary>
		/// <param name="responseCode">
		/// The HTTP status code returned by the receiver.
		/// </param>
		/// <param name="responseMessage">
		/// An optional message returned by the receiver.
		/// </param>
		public void Complete(int responseCode, string? responseMessage = null) {
			ResponseCode = responseCode;
			ResponseMessage = responseMessage;
			CompletedAt = DateTimeOffset.UtcNow;
		}

		/// <summary>
		/// Signals that the attempt has failed locally.
		/// </summary>
		/// <param name="responseMessage">
		/// A message describing the failure.
		/// </param>
		public void LocalFail(string? responseMessage = null) {
			ResponseCode = null;
			ResponseMessage = responseMessage;
			CompletedAt = DateTimeOffset.UtcNow;
		}

		/// <summary>
		/// Signals that the attempt has failed due to a timeout.
		/// </summary>
		public void TimeOut() => Complete((int)HttpStatusCode.RequestTimeout, "Request Timeout");
	}
}
