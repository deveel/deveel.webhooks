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

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides a list of the possible error codes
	/// that can be returned by an operation of
	/// management of webhook subscriptions.
	/// </summary>
	public static class WebhookSubscriptionErrorCodes {
		/// <summary>
		/// An unknown error occurred while managing webhook subscriptions.
		/// </summary>
		public const string UnknownError = "WEBHOOK_SUBSCRIPTION_UNKNOWN_ERROR";

		/// <summary>
		/// The subscription is invalid.
		/// </summary>
		public const string SubscriptionInvalid = "WEBHOOK_SUBSCRIPTION_INVALID";

		/// <summary>
		/// The new status of the subscription is invalid.
		/// </summary>
		public const string InvalidStatus = "WEBHOOK_SUBSCRIPTION_STATUS_INVALID";

		/// <summary>
		/// The subscription was not found.
		/// </summary>
		public const string SubscriptionNotFound = "WEBHOOK_SUBSCRIPTION_NOT_FOUND";
	}
}
