﻿// Copyright 2022-2023 Deveel
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

using System.Text.Json.Serialization;

using Deveel.Webhooks.Json;

namespace Deveel.Webhooks.SendGrid {
	public sealed class SendGridEmail {
		[JsonPropertyName("envelope")]
		public Envelope? Envelope { get; set; }

		[JsonPropertyName("from")]
		public EmailAddress From { get; set; }

		[JsonPropertyName("subject")]
		public string Subject { get; set; }

		[JsonPropertyName("to")]
		public IList<EmailAddress> To { get; set; }

		[JsonPropertyName("cc")]
		public IList<EmailAddress>? Cc { get; set; }

		[JsonPropertyName("bcc")]
		public IList<EmailAddress>? Bcc { get; set; }

		[JsonPropertyName("text")]
		public string? Text { get; set; }

		[JsonPropertyName("html")]
		[JsonConverter(typeof(Base64StringConverter))]
		public string? Html { get; set; }

		[JsonPropertyName("attachments")]
		public IList<EmailAttachment>? Attachments { get; set; }

		[JsonPropertyName("headers")]
		public IDictionary<string, string>? Headers { get; set; }

		[JsonPropertyName("dkim")]
		public string? Dkim { get; set; }

		[JsonPropertyName("spf")]
		public string? Spf { get; set; }

		[JsonPropertyName("spam_score")]
		public double? SpamScore { get; set; }

		[JsonPropertyName("sender_ip")]
		public string SenderIpAddress { get; set; }

		[JsonPropertyName("spam_report")]
		public EmailSpamReport? SpamReport { get; set; }
	}
}
