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

using System.Text;

using Deveel.Webhooks.SendGrid;

using Microsoft.AspNetCore.Http;

namespace Deveel.Webhooks {
	class SendGridEmailFormParser : IWebhookFormParser<SendGridEmail> {
		public Task<SendGridEmail> ParseWebhookAsync(IFormCollection form, CancellationToken cancellationToken) {
			var email = new SendGridEmail();

			if (form.TryGetValue("to", out var to)) {
				email.To = new List<EmailAddress>();
				foreach (var item in to) {
					if (EmailAddress.TryParse(item, out var emailAddress))
						email.To.Add(emailAddress);
				}
			}
			if (form.TryGetValue("cc", out var cc)) {
				email.Cc = new List<EmailAddress>();

				foreach (var item in cc) 
					if (EmailAddress.TryParse(item, out var emailAddress))
						email.Cc.Add(emailAddress);
			}
			if (form.TryGetValue("bcc", out var bcc)) {
				// TODO: parse multiple addresses
				email.Bcc = new List<EmailAddress>();
				foreach (var item in bcc)
					if (EmailAddress.TryParse(item, out var emailAddress))
						email.Bcc.Add(emailAddress);
			}
			if (form.TryGetValue("from", out var from)) {
				// TODO: parse the name ...
				if (EmailAddress.TryParse(from, out var address))
					email.From = address;
			}
			if (form.TryGetValue("subject", out var subject)) {
				email.Subject = subject;
			}
			if (form.TryGetValue("text", out var text)) {
				email.Text = text;
			}
			if (form.TryGetValue("html", out var html)) {
				email.Html = Encoding.UTF8.GetString(Convert.FromBase64String(html.ToString()));
			}
			if (form.TryGetValue("attachments", out var attachments)) {
				email.Attachments = new List<EmailAttachment>();

				foreach (var attachment in attachments) {
					var attachmentParts = attachment?.Split(";", StringSplitOptions.RemoveEmptyEntries);
					if (attachmentParts?.Length != 3)
						continue;

					var filename = attachmentParts[0];
					var type = attachmentParts[1];
					var content = attachmentParts[2];

					email.Attachments.Add(new EmailAttachment(filename, type, content));
				}
			}

			return Task.FromResult(email);
		}
	}
}
