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
using System.Diagnostics;
using System.Net;

namespace Deveel.Webhooks {
	/// <summary>
	/// Describes a single attempt in a
	/// process of delivery of a webhook.
	/// </summary>
	public sealed class WebhookDeliveryAttempt : IWebhookDeliveryAttempt {
		private readonly Stopwatch stopwatch;

		/// <summary>
		/// Constructs the attempt desciptor
		/// </summary>
		/// <remarks>
		/// Invoking this constrcutor also starts a timer
		/// for the count of the following reports.
		/// </remarks>
		public WebhookDeliveryAttempt() {
			stopwatch = new Stopwatch();
			stopwatch.Start();
			StartedAt = DateTimeOffset.UtcNow;
		}

		/// <summary>
		/// Gets a value indicating if any response was
		/// obtained by the receiver.
		/// </summary>
		public bool HasResponse => ResponseStatusCode != null;

		/// <summary>
		/// If a response was obtained by the receiver, this
		/// gets the HTTP status code of the response.
		/// </summary>
		public int? ResponseStatusCode { get; private set; }

		/// <summary>
		/// If a response was obtained by the receiver, this
		/// gets the message provided within the response.
		/// </summary>
		public string ResponseMessage { get; private set; }

		/// <summary>
		/// Gets the exact time-stamp at which the attempt
		/// was started.
		/// </summary>
		public DateTimeOffset StartedAt { get; }

		/// <summary>
		/// Gets the exact time-stamp at which the attempt
		/// was finished, of <strong>null</strong> if the
		/// attempt is still ongoing.
		/// </summary>
		public DateTimeOffset? EndedAt { get; private set; }

		/// <summary>
		/// Gets a value indicating if the attempt was unsuccessful.
		/// </summary>
		public bool Failed => ResponseStatusCode >= 400 || HasTimedOut;

		/// <summary>
		/// Gets a value indicating if the system timed-out while
		/// attempting to deliver a webhook.
		/// </summary>
		public bool HasTimedOut { get; private set; }

		/// <summary>
		/// Notifies that the attempt of delivery was finished
		/// with the given result.
		/// </summary>
		/// <param name="statusCode">The final HTTP status code returned by
		/// the receiver.</param>
		/// <param name="message">A message that describes the status
		/// of delivery of the webhook.</param>
		public void Finish(int statusCode, string message) {
			stopwatch.Stop();

			ResponseStatusCode = statusCode;
			ResponseMessage = message;
			EndedAt = StartedAt.Add(stopwatch.Elapsed);
		}

		/// <summary>
		/// Signals that the attempt of delivery timed-out.
		/// </summary>
		public void Timeout()
			=> Finish((int)HttpStatusCode.RequestTimeout, null);
	}
}
