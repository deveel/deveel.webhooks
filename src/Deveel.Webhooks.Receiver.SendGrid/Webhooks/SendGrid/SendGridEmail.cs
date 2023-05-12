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

using Deveel.Webhooks.Json;

namespace Deveel.Webhooks.SendGrid {
	/// <summary>
	/// Represents an email that is delivered to a receiver
	/// by SendGrid.
	/// </summary>
	public sealed class SendGridEmail {
		/// <summary>
		/// Gets or sets an envelope that describes the 
		/// email metadata.
		/// </summary>
		[JsonPropertyName("envelope")]
		public Envelope? Envelope { get; set; }

		/// <summary>
		/// Gets or sets the email address of the sender.
		/// </summary>
		[JsonPropertyName("from")]
		public EmailAddress From { get; set; }

		/// <summary>
		/// Gets or sets the subject of the email.
		/// </summary>
		[JsonPropertyName("subject")]
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the list of email addresses that
		/// are the main recipients of the email.
		/// </summary>
		[JsonPropertyName("to")]
		public IList<EmailAddress> To { get; set; }

		/// <summary>
		/// Gets or sets the list of email addresses that
		/// are the CC recipients of the email.
		/// </summary>
		[JsonPropertyName("cc")]
		public IList<EmailAddress>? Cc { get; set; }

		/// <summary>
		/// Gets or sets the list of email addresses that
		/// are the BCC recipients of the email.
		/// </summary>
		[JsonPropertyName("bcc")]
		public IList<EmailAddress>? Bcc { get; set; }

		/// <summary>
		/// Gets or sets the plain text content of the email.
		/// </summary>
		[JsonPropertyName("text")]
		public string? Text { get; set; }

		/// <summary>
		/// Gets or sets the HTML content of the email.
		/// </summary>
		[JsonPropertyName("html")]
		[JsonConverter(typeof(Base64StringConverter))]
		public string? Html { get; set; }

		/// <summary>
		/// Gets or sets the list of attachments of the email.
		/// </summary>
		[JsonPropertyName("attachments")]
		public IList<EmailAttachment>? Attachments { get; set; }

		/// <summary>
		/// Gets or sets the list of headers of the email.
		/// </summary>
		[JsonPropertyName("headers")]
		public IDictionary<string, string>? Headers { get; set; }

		/// <summary>
		/// Gets or sets the DKIM signature of the email.
		/// </summary>
		[JsonPropertyName("dkim")]
		public string? Dkim { get; set; }

		/// <summary>
		/// Gets or sets the SPF signature of the email.
		/// </summary>
		[JsonPropertyName("spf")]
		public string? Spf { get; set; }

		/// <summary>
		/// Gets or sets the spam score of the email.
		/// </summary>
		[JsonPropertyName("spam_score")]
		public double? SpamScore { get; set; }

		/// <summary>
		/// Gets or sets the IP address of the sender
		/// of the email.
		/// </summary>
		[JsonPropertyName("sender_ip")]
		public string? SenderIpAddress { get; set; }

		/// <summary>
		/// Gets or sets the spam report of the email.
		/// </summary>
		[JsonPropertyName("spam_report")]
		public EmailSpamReport? SpamReport { get; set; }
	}
}
