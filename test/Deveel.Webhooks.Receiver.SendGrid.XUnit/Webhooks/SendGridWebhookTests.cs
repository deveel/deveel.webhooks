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

using Deveel.Webhooks.SendGrid;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public class SendGridWebhookTests : ReceiverTestBase<SendGridWebhook> {
		public SendGridWebhookTests(ITestOutputHelper outputHelper) : base(outputHelper) {
		}

		//public IWebhookReceiver<SendGridWebhook> Receiver => Services.GetRequiredService<IWebhookReceiver<SendGridWebhook>>();

		protected override void AddReceiver(IServiceCollection services) {
			services.AddSendGridReceiver(new SendGridReceiverOptions {
				VerifySignature = true,
				Secret = "abc1234567890"
			});
		}

		protected override HttpRequest CreateRequestWithJson(string path, string json) {
			var request = base.CreateRequestWithJson(path, json);

			request.Headers["X-Twilio-Email-Event-Webhook-Signature"] = 
				SendGridWebhookSignature.Create(json, "abc1234567890");

			return request;
		}

		[Fact]
		public async Task ReceiveStatusWithInvalidSignature() {
			var request = CreateRequestWithJson(new[] {
				new {
					email = "foo@bar.com",
					timestamp = 1612345678,
					@event = "processed",
					category = new[] { "foo" },
					sg_event_id = Guid.NewGuid().ToString(),
					sg_message_id = Guid.NewGuid().ToString(),
					status = "processed",
					ip = "192.168.0.1"
					// TODO: find a way with anonymou types ...
					// smtp-id = "<14c5d75ce93.dfd.64b469@ismtpd-555>"
				}
			});

			request.Headers["X-Twilio-Email-Event-Webhook-Signature"]
				= "sha256=abc1234567890";

			var result = await Receiver.ReceiveAsync(request);

			Assert.False(result.Successful);
			Assert.False(result.SignatureValid);
		}

		[Fact]
		public async Task ReceiveEmailProcessedWebhook() {
			var request = CreateRequestWithJson(new[] {
				new {
					email = "foo@bar.com",
					timestamp = 1612345678,
					@event = "processed",
					category = new[] { "foo" },
					sg_event_id = Guid.NewGuid().ToString(),
					sg_message_id = Guid.NewGuid().ToString(),
					status = "processed",
					ip = "192.168.0.1",
					ip_pool = "Pool 0",
					ip_address = "255.255.255.0",
					tls = true,

					// TODO: find a way with anonymou types ...
					// smtp-id = "<14c5d75ce93.dfd.64b469@ismtpd-555>"
				}
			});

			var result = await Receiver.ReceiveAsync(request);

			Assert.True(result.Successful);

			var webhooks = result.Webhooks;
			Assert.NotNull(webhooks);
			Assert.NotEmpty(webhooks);
			Assert.Single(webhooks);

			var webhook = webhooks[0];
			Assert.NotNull(webhook);
			Assert.Equal("foo@bar.com", webhook.Email);
			Assert.Equal(1612345678, webhook.TimeStamp.ToUnixTimeSeconds());
			Assert.Equal(SendGridEventType.Processed, webhook.EventType);
			Assert.Equal(SendGridEmailStatus.Processed, webhook.Status);
			Assert.NotNull(webhook.Categories);
			Assert.Contains("foo", webhook.Categories);
			Assert.NotNull(webhook.EventId);
			Assert.NotNull(webhook.MessageId);
			Assert.Equal("192.168.0.1", webhook.ClientIpAddress);
			Assert.Equal("Pool 0", webhook.IpPoolName);
			Assert.Equal("255.255.255.0", webhook.SenderIpAddress);
		}

		[Fact]
		public async Task ReceiveEmailDeliveredWebhook() {
			var request = CreateRequestWithJson(new[] {
				new {
					email = "foo@bar.com",
					timestamp = 1612345678,
					@event = "delivered",
					category = new[] { "foo" },
					sg_event_id = Guid.NewGuid().ToString(),
					sg_message_id = Guid.NewGuid().ToString(),
					status = "delivered",
					ip = "255.255.255.0",
					tls = true,
					cert_err = false
				}
			});

			var result = await Receiver.ReceiveAsync(request);
			Assert.True(result.Successful);

			Assert.NotNull(result.Webhooks);

			var webhook = result.Webhooks[0];
			Assert.NotNull(webhook);
			Assert.Equal("foo@bar.com", webhook.Email);
			Assert.Equal(1612345678, webhook.TimeStamp.ToUnixTimeSeconds());
			Assert.Equal(SendGridEventType.Delivered, webhook.EventType);
			Assert.Equal(SendGridEmailStatus.Delivered, webhook.Status);
			Assert.NotNull(webhook.Categories);
			Assert.Contains("foo", webhook.Categories);
			Assert.NotNull(webhook.EventId);
			Assert.NotNull(webhook.MessageId);
			Assert.Equal("255.255.255.0", webhook.ClientIpAddress);
			Assert.True(webhook.Tls);
		}

		[Fact]
		public async Task ReceiveBouncedEmailWebhook() {
			var request = CreateRequestWithJson(new[] {
				new {
					email = "foo@bar.com",
					timestamp = 1612345678,
					@event = "bounce",
					category = new[] { "foo" },
					sg_event_id = Guid.NewGuid().ToString(),
					sg_message_id = Guid.NewGuid().ToString(),
					status = "bounce",
					reason = "500 unknown recipient",
					bound_class = "25",
					asm_group_id = "8599494994",
					attempt = 1,
					ip = "192.168.0.1",
					tls = true,
					response = "403",
					response_phrase = "Forbidden",
					user_agent = "Mozilla/4.0",
				}
			});

			var result = await Receiver.ReceiveAsync(request);

			Assert.True(result.Successful);
			Assert.NotNull(result.Webhooks);

			var webhook = result.Webhooks[0];

			Assert.NotNull(webhook);
			Assert.Equal("foo@bar.com", webhook.Email);
			Assert.Equal(1612345678, webhook.TimeStamp.ToUnixTimeSeconds());
			Assert.Equal(SendGridEventType.Bounced, webhook.EventType);
			Assert.Equal(SendGridEmailStatus.Bounce, webhook.Status);
			Assert.NotNull(webhook.Categories);
			Assert.Contains("foo", webhook.Categories);
			Assert.NotNull(webhook.EventId);
			Assert.NotNull(webhook.MessageId);
			Assert.Equal("500 unknown recipient", webhook.Reason);
		}

		[Fact]
		public async Task ReceiveSingleWebhook() {
			var request = CreateRequestWithJson(new {
				email = "foo@bar.com",
				timestamp = 1612345678,
				@event = "processed",
				category = new[] { "foo" },
				sg_event_id = Guid.NewGuid().ToString(),
				sg_message_id = Guid.NewGuid().ToString(),
				status = "processed"
			});

			await Assert.ThrowsAsync<WebhookParseException>(() => Receiver.ReceiveAsync(request));
		}
	}
}
