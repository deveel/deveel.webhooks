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

using System.Runtime.Serialization;

namespace Deveel.Webhooks.SendGrid {
	/// <summary>
	/// Represents the type of events notified by a SendGrid webhook.
	/// </summary>
	[DataContract]
	public enum SendGridEventType {
		/// <summary>
		/// The type of the event is unknown.
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// The email has been successfully processed by SendGrid.
		/// </summary>
		[EnumMember(Value = "processed")]
		Processed,

		/// <summary>
		/// The email could not be delivered immediately and has been temporarily deferred by SendGrid.
		/// </summary>
		[EnumMember(Value = "deferred")]
		Deferred,

		/// <summary>
		/// The email has been successfully delivered to the recipient's email server.
		/// </summary>
		[EnumMember(Value = "delivered")]
		Delivered,

		/// <summary>
		/// The recipient has opened the email.
		/// </summary>
		[EnumMember(Value = "open")]
		Opened,

		/// <summary>
		/// The recipient has clicked a link in the email.
		/// </summary>
		[EnumMember(Value = "click")]
		Clicked,

		/// <summary>
		/// The email has bounced and could not be delivered to the recipient.
		/// </summary>
		[EnumMember(Value = "bounce")]
		Bounced,

		/// <summary>
		/// The email has been dropped by SendGrid due to a policy violation or other issue.
		/// </summary>
		[EnumMember(Value = "dropped")]
		Dropped,

		/// <summary>
		/// The recipient has marked the email as spam.
		/// </summary>
		[EnumMember(Value = "spamreport")]
		SpamReported,

		/// <summary>
		/// The recipient has unsubscribed from the email.
		/// </summary>
		[EnumMember(Value = "unsubscribe")]
		Unsubscribed,

		/// <summary>
		/// The recipient has unsubscribed from a specific email group.
		/// </summary>
		[EnumMember(Value = "group_unsubscribe")]
		GroupUnsubscribed,

		/// <summary>
		/// The recipient has resubscribed to a specific email group.
		/// </summary>
		[EnumMember(Value = "group_resubscribe")]
		GroupResubscribed,

		/// <summary>
		/// The email has been successfully processed by SendGrid and was an inbound message.
		/// </summary>
		[EnumMember(Value = "processed:inbound")]
		ProcessedInbound
	}
}
