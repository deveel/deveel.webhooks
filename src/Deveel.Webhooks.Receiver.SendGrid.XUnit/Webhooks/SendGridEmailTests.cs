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

using System.Reflection.Metadata;
using System.Text;

using Deveel.Webhooks.SendGrid;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public sealed class SendGridEmailTests : ReceiverTestBase<SendGridEmail> {
		public SendGridEmailTests(ITestOutputHelper outputHelper) : base(outputHelper) {
		}

		protected override void AddReceiver(IServiceCollection services) {
			services.AddSendGridEmailReceiver();
		}

		private async Task<WebhookReceiveResult<SendGridEmail>> ReceiveEmailAsync(object content) {
			var request = CreateRequestWithJson(content);

			return await Receiver.ReceiveAsync(request);
		}

		private async Task<WebhookReceiveResult<SendGridEmail>> ReceiveFormEmailAsync(Dictionary<string, StringValues> form) {
			var request = CreateRequestWithForm(form);
			return await ReceiveAsync(request);
		}

		[Fact]
		public async Task ReceiveTextEmail() {
			var result = await ReceiveEmailAsync(new {
				from = new {
					email = "joe@example.com",
					name = "John Doe"
				},
				to = new[] {
					new {
						email = "info@foobar.com"
					}
				},
				subject = "Hello, World!",
				text = "This is a test email",
			});

			Assert.True(result.Successful);
			Assert.NotNull(result.Webhooks);
			Assert.NotEmpty(result.Webhooks);
			Assert.Single(result.Webhooks);

			var webhook = result.Webhooks[0];
			Assert.NotNull(webhook);
			Assert.Equal("joe@example.com", webhook.From.Address);
			Assert.Equal("John Doe", webhook.From.Name);
			Assert.NotNull(webhook.To);
			Assert.NotEmpty(webhook.To);
			Assert.Single(webhook.To);
			Assert.Equal("info@foobar.com", webhook.To[0].Address);
			Assert.Null(webhook.To[0].Name);
			Assert.Equal("Hello, World!", webhook.Subject);
			Assert.Equal("This is a test email", webhook.Text);
			Assert.Null(webhook.Html);
		}

		[Fact]
		public async Task ReceiveHtmlEmail() {
			var result = await ReceiveEmailAsync(new {
				from = new {
					email = "joe@example.com",
					name = "John Doe"
				},
				to = new[] {
					new {
						email = "info@foobar.com"
					}
				},
				subject = "Hello, World!",
				html = Convert.ToBase64String(Encoding.UTF8.GetBytes("<html><body><h1>This is a test email</h1></body></html>")),
			});

			Assert.True(result.Successful);
			Assert.NotNull(result.Webhooks);
			Assert.NotEmpty(result.Webhooks);
			Assert.Single(result.Webhooks);

			var webhook = result.Webhooks[0];
			Assert.NotNull(webhook);
			Assert.Equal("joe@example.com", webhook.From.Address);
			Assert.Equal("John Doe", webhook.From.Name);
			Assert.NotNull(webhook.To);
			Assert.NotEmpty(webhook.To);
			Assert.Single(webhook.To);
			Assert.Equal("info@foobar.com", webhook.To[0].Address);
			Assert.Null(webhook.To[0].Name);
			Assert.Equal("Hello, World!", webhook.Subject);
			Assert.Null(webhook.Text);
			Assert.Equal("<html><body><h1>This is a test email</h1></body></html>", webhook.Html);
		}

		[Fact]
		public async Task ReceiveEmailWithSpamReport() {
			var result = await ReceiveEmailAsync(new {
				from = new {
					email = "joe@example.com",
					name = "John Doe"
				},
				to = new[] {
					new {
						email = "info@example.com"
					}
				},
				subject = "Hello, World!",
				text = "This is a test email",
				spam_report = new {
					score = 3,
					threshold = 5,
					matched_rules = new[] {
						new {
							name = "rule1",
							description = "Rule 1"
						},
						new {
							name = "rule2",
							description = "Rule 2"
						}
					},
					spam_report = new {
						score = 3,
						threshold = 5,
						rules = new[] {
							new {
								name = "rule1",
								description = "Rule 1",
								score = 2
							},
							new {
								name = "rule2",
								description = "Rule 2",
								score = 1
							}
						}
					}
				}
			});

			Assert.True(result.Successful);

			Assert.NotNull(result.Webhooks);
			Assert.NotEmpty(result.Webhooks);
			Assert.Single(result.Webhooks);

			var webhook = result.Webhooks[0];

			Assert.NotNull(webhook);
			Assert.NotNull(webhook.SpamReport);
			Assert.Equal(3, webhook.SpamReport.Score);
			Assert.Equal(5, webhook.SpamReport.Threshold);
			Assert.NotNull(webhook.SpamReport.MatchedRules);
			Assert.NotEmpty(webhook.SpamReport.MatchedRules);
			Assert.Equal(2, webhook.SpamReport.MatchedRules.Count);
			Assert.Equal("rule1", webhook.SpamReport.MatchedRules[0].Name);
			Assert.Equal("Rule 1", webhook.SpamReport.MatchedRules[0].Description);
			Assert.Equal("rule2", webhook.SpamReport.MatchedRules[1].Name);
			Assert.Equal("Rule 2", webhook.SpamReport.MatchedRules[1].Description);
			Assert.NotNull(webhook.SpamReport.Details);
			Assert.Equal(3, webhook.SpamReport.Details.Score);
			Assert.Equal(5, webhook.SpamReport.Details.Threshold);
			Assert.NotNull(webhook.SpamReport.Details.Rules);
			Assert.NotEmpty(webhook.SpamReport.Details.Rules);
			Assert.Equal(2, webhook.SpamReport.Details.Rules.Count);
			Assert.Equal("rule1", webhook.SpamReport.Details.Rules[0].Name);
			Assert.Equal("Rule 1", webhook.SpamReport.Details.Rules[0].Description);
			Assert.Equal(2, webhook.SpamReport.Details.Rules[0].Score);
			Assert.Equal("rule2", webhook.SpamReport.Details.Rules[1].Name);
			Assert.Equal("Rule 2", webhook.SpamReport.Details.Rules[1].Description);
			Assert.Equal(1, webhook.SpamReport.Details.Rules[1].Score);
		}

		[Fact]
		public async Task ReceiveFormEmail() {
			var result = await ReceiveFormEmailAsync(new Dictionary<string, StringValues> {
				{"from", "\"Jonh Doe\"<joe@example.com>"},
				{"to", new StringValues(new[]{ "info@foobar.com","info2@foobar.com" }) },
				{"subject", "Hello, World!"},
				{"text", "This is a test email"},
				{"ip", "192.168.0.1" }
			});

			Assert.True(result.Successful);
			Assert.NotNull(result.Webhooks);
			Assert.NotEmpty(result.Webhooks);
			Assert.Single(result.Webhooks);

			var webhook = result.Webhooks[0];
			Assert.NotNull(webhook);
			Assert.Equal("joe@example.com", webhook.From.Address);
			Assert.Equal("Jonh Doe", webhook.From.Name);
			Assert.NotNull(webhook.To);
			Assert.NotEmpty(webhook.To);
			Assert.Equal(2, webhook.To.Count);
			Assert.Equal("info@foobar.com", webhook.To[0].Address);
			Assert.Equal("", webhook.To[0].Name);
			Assert.Equal("info2@foobar.com", webhook.To[1].Address);
			Assert.Equal("", webhook.To[1].Name);
			Assert.Equal("Hello, World!", webhook.Subject);
			Assert.Equal("This is a test email", webhook.Text);
		}
	}
}
