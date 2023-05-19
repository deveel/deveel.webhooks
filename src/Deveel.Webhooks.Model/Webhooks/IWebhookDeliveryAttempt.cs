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

namespace Deveel.Webhooks {
	/// <summary>
	/// Represents a single attempt to deliver a webhook notification
	/// to a remote receiver.
	/// </summary>
	public interface IWebhookDeliveryAttempt {
		/// <summary>
		/// Gets an HTTP status code that was returned by the remote
		/// server when the notification was attempted.
		/// </summary>
		int? ResponseStatusCode { get; }

		/// <summary>
		/// Gets a message that was returned by the remote server when
		/// the notification was attempted.
		/// </summary>
		string? ResponseMessage { get; }

		/// <summary>
		/// Gets the exact time the delivery attempt started.
		/// </summary>
		DateTimeOffset StartedAt { get; }

		/// <summary>
		/// Gets the exact time the delivery attempt ended,
		/// if it was able to be completed.
		/// </summary>
		DateTimeOffset? EndedAt { get; }
	}
}
