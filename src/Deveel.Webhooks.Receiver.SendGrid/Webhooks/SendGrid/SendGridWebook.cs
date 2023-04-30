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

// Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618

using System.Text.Json.Serialization;

using Deveel.Json;

namespace Deveel.Webhooks.SendGrid {
	/// <summary>
	/// Represents a webhook event from SendGrid, that informs
	/// of a change in the status of an email message.
	/// </summary>
	public sealed class SendGridWebhook {
		/// <summary>
		/// Gets or sets the email address of the recipient.
		/// </summary>
		[JsonPropertyName("email")]
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the timestamp of the event in Unix seconds.
		/// </summary>
		[JsonPropertyName("timestamp")]
		[JsonConverter(typeof(UnixTimeSecondsConverter))]
		public DateTimeOffset TimeStamp { get; set; }

		/// <summary>
		/// Gets or sets the unique identifier of the email message as
		/// assigned by the SendGrid SMTP server.
		/// </summary>
		[JsonPropertyName("smtp-id")]
		public string SmtpId { get; set; }

		/// <summary>
		/// Gets or sets the type of the event.
		/// </summary>
		[JsonPropertyName("event")]
		public SendGridEventType EventType { get; set; }

		/// <summary>
		/// Gets or sets the categories of the email message,
		/// that can be used to filter the events.
		/// </summary>
		[JsonPropertyName("category")]
		public IList<string>? Categories { get; set; }

		/// <summary>
		/// Gets or sets the unique identifier of the event.
		/// </summary>
		[JsonPropertyName("sg_event_id")]
		public string EventId { get; set; }

		/// <summary>
		/// Gets or sets the unique identifier of the message.
		/// </summary>
		[JsonPropertyName("sg_message_id")]
		public string MessageId { get; set; }

		/// <summary>
		/// Gets or sets a response message from the recipient's email server.
		/// </summary>
		[JsonPropertyName("response")]
		public string? Response { get; set; }

		/// <summary>
		/// Gets or sets the number of times SendGrid has attempted to deliver the email.
		/// </summary>
		[JsonPropertyName("attempt")]
		public int? Attempts { get; set; }

		/// <summary>
		/// Gets or sets the user-agent of the recipient's email client.
		/// </summary>
		[JsonPropertyName("useragent")]
		public string? UserAgent { get; set; }

		/// <summary>
		/// Gets or sets the IP address of the recipient that triggered the event.
		/// </summary>
		[JsonPropertyName("ip")]
		public string? ClientIpAddress { get; set; }

		/// <summary>
		/// Gets or sets the name of the IP pool that the email was sent from.
		/// </summary>
		[JsonPropertyName("ip_pool")]
		public string? IpPoolName { get; set; }

		/// <summary>
		/// Gets or sets the address of that sent the email.
		/// </summary>
		[JsonPropertyName("ip_address")]
		public string? SenderIpAddress { get; set; }

		/// <summary>
		/// Gets or sets the URL that was clicked and triggered this event.
		/// </summary>
		[JsonPropertyName("url")]
		public string? Url { get; set; }

		/// <summary>
		/// Gets or sets the reason why the event occurred.
		/// </summary>
		[JsonPropertyName("reason")]
		public string? Reason { get; set; }

		/// <summary>
		/// Gets or sets the reason code for the event.
		/// </summary>
		[JsonPropertyName("reason_code")]
		public string? ReasonCode { get; set; }

		/// <summary>
		/// Gets or sets the status of the email message.
		/// </summary>
		[JsonPropertyName("status")]
		public SendGridEmailStatus Status { get; set; }

		/// <summary>
		/// Gets or sets the status code returned by the recipient's email server.
		/// </summary>
		[JsonPropertyName("status_code")]
		public string? StatusCode { get; set; }

		/// <summary>
		/// Gets or sets a flag indicating whether TLS was used in the 
		/// transmission of the email
		/// </summary>
		[JsonPropertyName("tls")]
		public bool? Tls { get; set; }

		/// <summary>
		/// Gets or sets the certificate error that occurred, if any.
		/// </summary>
		[JsonPropertyName("cert_err")]
		public bool? CertificateError { get; set; }

		[JsonPropertyName("cert_issuer")]
		public string CertIssuer { get; set; }

		[JsonPropertyName("cert_subject")]
		public string CertSubject { get; set; }

		[JsonPropertyName("cert_verified")]
		public string CertVerified { get; set; }

		[JsonPropertyName("url_offset")]
		public string UrlOffset { get; set; }

		/// <summary>
		/// Gets or sets the identifier of the unsubscribe group the email recipient 
		/// is a member of.
		/// </summary>
		[JsonPropertyName("asm_group_id")]
		public string? AsmGroupId { get; set; }

		/// <summary>
		/// Gets or sets The identifier of the AMP template used for the email.
		/// </summary>
		[JsonPropertyName("asm_template_id")]
		public string? AsmpTemplateId { get; set; }

		/// <summary>
		/// Gets or sets the identifier of batch that the email 
		/// was a part of.
		/// </summary>
		[JsonPropertyName("batch_id")]
		public string? BatchId { get; set; }
	}
}
