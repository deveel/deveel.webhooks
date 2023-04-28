using System.Security.Cryptography;
using System.Text;

using Deveel.Webhooks.Twilio;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public sealed class TwilioReceiverTests {
		public TwilioReceiverTests(ITestOutputHelper outputHelper) {
			var services = new ServiceCollection();
			ConfigureServices(services);

			Services = services.BuildServiceProvider();
		}

		private void ConfigureServices(IServiceCollection services) {
			services.AddTwilioReceiver(options => {
				options.VerifySignature = true;
				options.AuthToken = "1234567890";
			});
		}

		private IServiceProvider Services { get; }

		public IWebhookReceiver<TwilioWebhook> Receiver => Services.GetRequiredService<IWebhookReceiver<TwilioWebhook>>();

		private static void SetSignature(HttpRequest request, string authToken) {
			request.Headers["X-Twilio-Signature"] = TwilioSignature.Create(request, authToken);
		}

		private HttpContext CreateContext(Dictionary<string, StringValues> form) {
			var context = new DefaultHttpContext();
			context.Request.Scheme = "https";
			context.Request.Headers.Host = "example.com";
			context.Request.Method = "POST";
			context.Request.Path = "/webhook/twilio";
			context.Request.Form = new FormCollection(form);
			context.Request.ContentType = "application/x-www-form-urlencoded";

			SetSignature(context.Request, "1234567890");

			return context;
		}

		private HttpContext CreateContextWithInvalidSignature(Dictionary<string, StringValues> form) {
			var context = CreateContext(form);
			context.Request.Headers["X-Twilio-Signature"] = "invalid-signature";
			return context;
		}

		[Fact]
		public async Task ReceiveSimpleSms() {
			var context = CreateContext(new Dictionary<string, StringValues> {
				{ "SmsSid", "SM1234567890" },
				{ "SmsStatus", "received" },
				{ "Body", "Hello World" },
				{ "From", "+1234567890" },
				{ "To", "+0987654321"  }
			});

			var result = await Receiver.ReceiveAsync(context.Request);

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
			var context = CreateContext(new Dictionary<string, StringValues> {
				{ "SmsSid", "SM1234567890" },
				{ "SmsStatus", "received" },
				{ "Body", "Hello World" },
				{ "From", "+1234567890" },
				{ "To", "+0987654321"  },
				{ "NumMedia", "1" },
				{ "MediaUrl0", "https://example.com/image.jpg" },
				{ "MediaContentType0", "image/jpeg" }
			});

			var result = await Receiver.ReceiveAsync(context.Request);

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
			var context = CreateContext(new Dictionary<string, StringValues> {
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

			var result = await Receiver.ReceiveAsync(context.Request);

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
			var context = CreateContext(new Dictionary<string, StringValues> {
				{ "SmsSid", "SM1234567890" },
				{ "SmsStatus", "failed" },
				{ "ErrorCode", "30001" },
				{ "ErrorMessage", "Unknown error" },
				{ "From", "+1234567890" },
				{ "To", "+0987654321"  }
			});

			var result = await Receiver.ReceiveAsync(context.Request);
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
			var context = CreateContextWithInvalidSignature(new Dictionary<string, StringValues> {
				{ "SmsSid", "SM1234567890" },
				{ "SmsStatus", "received" },
				{ "Body", "Hello World" },
				{ "From", "+1234567890" },
				{ "To", "+0987654321"  }
			});

			var result = await Receiver.ReceiveAsync(context.Request);
			Assert.False(result.Successful);
			Assert.Null(result.Webhook);
		}
	}
}
