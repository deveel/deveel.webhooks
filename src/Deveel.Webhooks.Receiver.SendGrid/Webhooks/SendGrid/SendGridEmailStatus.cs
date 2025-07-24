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

using System.Runtime.Serialization;

namespace Deveel.Webhooks.SendGrid {

	/// <summary>
	/// Represents the possible statuses of an email delivery 
	/// status updates by SendGrid.
	/// </summary>
	public enum SendGridEmailStatus {
		/// <summary>
		/// The status of the email is unknown.
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// The email has been processed by SendGrid.
		/// </summary>
		[EnumMember(Value = "processed")]
		Processed,

		/// <summary>
		/// The email has been dropped by SendGrid.
		/// </summary>
		[EnumMember(Value = "dropped")]
		Dropped,

		/// <summary>
		/// The email has been delivered to the recipient.
		/// </summary>
		[EnumMember(Value = "delivered")]
		Delivered,

		/// <summary>
		/// The email has bounced back to SendGrid.
		/// </summary>
		[EnumMember(Value = "bounce")]
		Bounce,

		/// <summary>
		/// The delivery of the email has been deferred by SendGrid.
		/// </summary>
		[EnumMember(Value = "deferred")]
		Deferred,

		/// <summary>
		/// The email has been opened by the recipient.
		/// </summary>
		[EnumMember(Value = "open")]
		Open,

		/// <summary>
		/// The email has been clicked by the recipient.
		/// </summary>
		[EnumMember(Value = "click")]
		Click,

		/// <summary>
		/// The email has been marked as spam by the recipient.
		/// </summary>
		[EnumMember(Value = "spamreport")]
		SpamReport,

		/// <summary>
		/// The recipient has unsubscribed from the email list.
		/// </summary>
		[EnumMember(Value = "unsubscribe")]
		Unsubscribe,

		/// <summary>
		/// The recipient has unsubscribed from a group email list.
		/// </summary>
		[EnumMember(Value = "group_unsubscribe")]
		GroupUnsubscribe,

		/// <summary>
		/// The recipient has resubscribed to a group email list.
		/// </summary>
		[EnumMember(Value = "group_resubscribe")]
		GroupResubscribe
	}
}
