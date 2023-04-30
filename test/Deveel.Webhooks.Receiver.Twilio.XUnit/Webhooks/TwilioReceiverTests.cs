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

using Deveel.Webhooks.Twilio;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public sealed class TwilioReceiverTests : ReceiverTestBase<TwilioWebhook> {
		public TwilioReceiverTests(ITestOutputHelper outputHelper) : base(outputHelper) {
		}

		protected override void AddReceiver(IServiceCollection services) {
			services.AddTwilioReceiver(new TwilioReceiverOptions {
				VerifySignature = true,
				AuthToken = "1234567890"
			});
		}

		private static void SetSignature(HttpRequest request, string authToken) {
			SetSignatureHeader(request, TwilioSignature.Create(request, authToken));
		}

		private static void SetSignatureHeader(HttpRequest request, string signature) {
			request.Headers["X-Twilio-Signature"] = signature;
		}

		protected override HttpRequest CreateRequestWithForm(string path, Dictionary<string, StringValues> form) {
			var request = base.CreateRequestWithForm(path, form);

			SetSignature(request, "1234567890");

			return request;
		}

		[Fact]
		public async Task ReceiveSimpleSms() {
			var request = CreateRequestWithForm(new Dictionary<string, StringValues> {
				{ "SmsSid", "SM1234567890" },
				{ "SmsStatus", "received" },
				{ "Body", "Hello World" },
				{ "From", "+1234567890" },
				{ "To", "+0987654321"  }
			});

			var result = await Receiver.ReceiveAsync(request);

			Assert.True(result.Successful);
			Assert.NotNull(result.Webhook);
			Assert.Equal("SM1234567890", result.Webhook.SmsId);
			Assert.Equal(MessageStatus.Received, result.Webhook.MessageStatus);
			Assert.Equal("Hello World", result.Webhook.Body);
			Assert.NotNull(result.Webhook.From);
			Assert.Equal("+1234567890", result.Webhook.From.PhoneNumber);
			Assert.NotNull(result.Webhook.To);
			Assert.Equal("+0987654321", result.Webhook.To.PhoneNumber);
		}

		[Fact]
		public async Task ReceiveSmsWithMedia() {
			var request = CreateRequestWithForm(new Dictionary<string, StringValues> {
				{ "SmsSid", "SM1234567890" },
				{ "SmsStatus", "received" },
				{ "Body", "Hello World" },
				{ "From", "+1234567890" },
				{ "To", "+0987654321"  },
				{ "NumMedia", "1" },
				{ "MediaUrl0", "https://example.com/image.jpg" },
				{ "MediaContentType0", "image/jpeg" }
			});

			var result = await Receiver.ReceiveAsync(request);

			Assert.True(result.Successful);
			Assert.NotNull(result.Webhook);
			Assert.Equal("SM1234567890", result.Webhook.SmsId);
			Assert.Equal(MessageStatus.Received, result.Webhook.MessageStatus);
			Assert.Equal("Hello World", result.Webhook.Body);
			Assert.NotNull(result.Webhook.From);
			Assert.Equal("+1234567890", result.Webhook.From.PhoneNumber);
			Assert.NotNull(result.Webhook.To);
			Assert.Equal("+0987654321", result.Webhook.To.PhoneNumber);
			Assert.NotNull(result.Webhook.Media);
			Assert.Single(result.Webhook.Media);
			Assert.Equal("https://example.com/image.jpg", result.Webhook.Media[0].Url);
			Assert.Equal("image/jpeg", result.Webhook.Media[0].ContentType);
		}

		[Fact]
		public async Task ReceiveSmsWithSegments() {
			var request = CreateRequestWithForm(new Dictionary<string, StringValues> {
				{ "SmsSid", "SM1234567890" },
				{ "SmsStatus", "received" },
				{ "Body", "Hello World" },
				{ "From", "+1234567890" },
				{ "To", "+0987654321"  },
				{ "NumSegments", "2" },
				{ "MessageSid", "MM1234567890" },
				{ "MessageSid0", "MM1234567891" },
				{ "Body0", "0" },
				{ "MessageSid1", "MM1234567892" },
				{ "Body1", "1" }
			});

			var result = await Receiver.ReceiveAsync(request);

			Assert.True(result.Successful);
			Assert.NotNull(result.Webhook);
			Assert.Equal("SM1234567890", result.Webhook.SmsId);
			Assert.Equal(MessageStatus.Received, result.Webhook.MessageStatus);
			Assert.Equal("Hello World", result.Webhook.Body);
			Assert.NotNull(result.Webhook.From);
			Assert.Equal("+1234567890", result.Webhook.From.PhoneNumber);
			Assert.NotNull(result.Webhook.To);
			Assert.Equal("+0987654321", result.Webhook.To.PhoneNumber);
			Assert.NotNull(result.Webhook.Segments);
			Assert.Equal(2, result.Webhook.Segments.Length);
			Assert.Equal(0, result.Webhook.Segments[0].Index);
			Assert.Equal("0", result.Webhook.Segments[0].Text);
			Assert.Equal(1, result.Webhook.Segments[1].Index);
			Assert.Equal("1", result.Webhook.Segments[1].Text);
		}

		[Fact]
		public async Task ReceiveErrorWebhook() {
			var request = CreateRequestWithForm(new Dictionary<string, StringValues> {
				{ "SmsSid", "SM1234567890" },
				{ "SmsStatus", "failed" },
				{ "ErrorCode", "30001" },
				{ "ErrorMessage", "Unknown error" },
				{ "From", "+1234567890" },
				{ "To", "+0987654321"  }
			});

			var result = await Receiver.ReceiveAsync(request);
			Assert.True(result.Successful);
			Assert.NotNull(result.Webhook);
			Assert.Equal("SM1234567890", result.Webhook.SmsId);
			Assert.Equal(MessageStatus.Failed, result.Webhook.MessageStatus);
			Assert.Equal("30001", result.Webhook.ErrorCode);
			Assert.Equal("Unknown error", result.Webhook.ErrorMessage);
			Assert.NotNull(result.Webhook.From);
			Assert.Equal("+1234567890", result.Webhook.From.PhoneNumber);
			Assert.NotNull(result.Webhook.To);
			Assert.Equal("+0987654321", result.Webhook.To.PhoneNumber);
		}

		[Fact]
		public async Task ReceiverSmsWithValidSignature() {
			var request = CreateRequestWithForm(new Dictionary<string, StringValues> {
				{ "SmsSid", "SM1234567890" },
				{ "SmsStatus", "received" },
				{ "Body", "Hello World" },
				{ "From", "+1234567890" },
				{ "To", "+0987654321"  }
			});

			SetSignatureHeader(request, "invalid");

			var result = await Receiver.ReceiveAsync(request);
			Assert.False(result.Successful);
			Assert.Null(result.Webhook);
		}

		[Fact]
		public async Task ReceiveSmsFromWhatsApp() {
			var request = CreateRequestWithForm(new Dictionary<string, StringValues> {
				{ "MessageSid", "SM1234567890" },
				{ "AccountSid", "AC1234567890" },
				{ "MessagingServiceSid", "MG1234567890" },
				{ "From", "whatsapp:+1234567890" },
				{ "To", "whatsapp:+0987654321" },
				{ "Body", "Hello World" }
			});

			var result = await Receiver.ReceiveAsync(request);
			Assert.True(result.Successful);
			Assert.NotNull(result.Webhook);
			Assert.NotNull(result.Webhook.From);
			Assert.True(result.Webhook.From.IsWhatsApp());
			Assert.Equal("+1234567890", result.Webhook.From.WhatsAppPhoneNumber);
			Assert.NotNull(result.Webhook.To);
			Assert.True(result.Webhook.To.IsWhatsApp());
			Assert.Equal("+0987654321", result.Webhook.To.WhatsAppPhoneNumber);
		}
	}
}
