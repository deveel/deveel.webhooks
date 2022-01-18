using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.Common.ExtensionFramework;
using Microsoft.VisualStudio.TestPlatform.Common.Interfaces;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Xunit;

namespace Deveel.Webhooks {
	public class WebhookReceiverTests {
		private readonly IWebhookReceiver<WebhookPayload> receiver;

		public WebhookReceiverTests() {
			var services = new ServiceCollection();
			services.AddWebhookReceivers(webhook => webhook
				.AddReceiver<WebhookPayload>());

			var provider = services.BuildServiceProvider();
			receiver = provider.GetRequiredService<IWebhookReceiver<WebhookPayload>>();
		}

		[Fact]
		public async Task ReceiveWebhookFromRequest() {
			var webhook = new WebhookPayload {
				WebhookName = "Test Webhook",
				EventId = Guid.NewGuid().ToString("N"),
				EventType = "event.occurred",
				Data = JObject.FromObject(new TestData {
					Key = "foo",
					Value = "bar"
				})
			};

			var json = JObject.FromObject(webhook);
			var stream = new MemoryStream();

			var writer = new StreamWriter(stream);
			var jsonWriter = new JsonTextWriter(writer);
					await json.WriteToAsync(jsonWriter);
					await jsonWriter.FlushAsync();

			stream.Position = 0;

			var context = new DefaultHttpContext();
			context.Request.ContentType = "application/json";
			context.Request.Method = "POST";
			context.Request.Body = stream;

			var received = await context.Request.GetWebhookAsync<WebhookPayload>();

			Assert.NotNull(received);
			Assert.Equal(webhook.EventId, received.EventId);
			Assert.Equal(webhook.EventType, received.EventType);
			Assert.NotNull(webhook.Data);
			Assert.Equal("foo", webhook.Data["Key"].Value<string>());
			Assert.Equal("bar", webhook.Data["Value"].Value<string>());
		}

		[Fact]
		public async Task ReceiveSignedWebhookFromRequest() {
			var webhook = new WebhookPayload {
				WebhookName = "Test Webhook",
				EventId = Guid.NewGuid().ToString("N"),
				EventType = "event.occurred",
				Data = JObject.FromObject(new TestData {
					Key = "foo",
					Value = "bar"
				})
			};

			var secret = Guid.NewGuid().ToString("N");

			var options = new WebhookReceiveOptions {
				Secret = secret,
				ValidateSignature = true
			};

			var json = JObject.FromObject(webhook);

			var signature = new Sha256WebhookSigner().Sign(json.ToString(Formatting.None), secret);

			var stream = new MemoryStream();

			var writer = new StreamWriter(stream);
			var jsonWriter = new JsonTextWriter(writer);
			await json.WriteToAsync(jsonWriter);
			await jsonWriter.FlushAsync();

			stream.Position = 0;

			var context = new DefaultHttpContext();
			context.Request.ContentType = "application/json";
			context.Request.Method = "POST";
			context.Request.QueryString = new QueryString($"?sig_alg=sha256&webhook-signature={signature}");
			context.Request.Body = stream;

			var received = await context.Request.GetWebhookAsync<WebhookPayload>(options);

			Assert.NotNull(received);
			Assert.Equal(webhook.EventId, received.EventId);
			Assert.Equal(webhook.EventType, received.EventType);
			Assert.NotNull(webhook.Data);
			Assert.Equal("foo", webhook.Data["Key"].Value<string>());
			Assert.Equal("bar", webhook.Data["Value"].Value<string>());
		}

		class TestData {
			public string Key { get; set; }

			public string Value { get; set; }
		}
	}
}
