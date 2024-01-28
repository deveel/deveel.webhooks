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

using System.Runtime.Serialization;

namespace Deveel.Webhooks {
	/// <summary>
	/// Enumerates the possible statuses of a webhook subscription
	/// </summary>
	public enum WebhookSubscriptionStatus {
		/// <summary>
		/// It is not possible to determine the status of the subscription
		/// </summary>
		[EnumMember(Value = "none")]
		None = 0,

		/// <summary>
		/// The subscription is active and can receive notifications
		/// </summary>
		[EnumMember(Value = "active")]
		Active = 1,

		/// <summary>
		/// The subscription is suspended and should not receive notifications
		/// </summary>
		[EnumMember(Value = "suspended")]
		Suspended = 2,

		/// <summary>
		/// The subscription has been cancelled and should not receive notifications
		/// </summary>
		[EnumMember(Value = "cancelled")]
		Cancelled = 3,
	}
}
