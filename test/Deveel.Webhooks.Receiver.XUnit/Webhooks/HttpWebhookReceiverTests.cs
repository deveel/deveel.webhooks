using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Xunit;

namespace Deveel.Webhooks {
	public class HttpWebhookReceiverTests {
		private readonly IHttpWebhookReceiver<WebhookPayload> httpReceiver;
		private bool signed;
		private WebhookSignatureLocation signatureLocation = WebhookSignatureLocation.QueryString;

		public HttpWebhookReceiverTests() {
			var services = new ServiceCollection();

			services.AddWebhookReceivers(webhook => webhook
				.Configure(options => {
					options.ValidateSignature = signed;
					options.SignatureLocation = signatureLocation;
				})
				.AddHttpReceiver<WebhookPayload>());

			var provider = services.BuildServiceProvider();
			httpReceiver = provider.GetRequiredService<IHttpWebhookReceiver<WebhookPayload>>();
		}

		[Fact]
		public async Task ReceiveWebhookFromHttpRequestMessage() {
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
			var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://callback.deveel.com");
			requestMessage.Content = new StringContent(json.ToString(Newtonsoft.Json.Formatting.None), Encoding.UTF8, "application/json");

			var received = await requestMessage.GetWebhookAsync<WebhookPayload>();

			Assert.NotNull(received);
			Assert.Equal(webhook.EventId, received.EventId);
			Assert.Equal(webhook.EventType, received.EventType);
			Assert.NotNull(webhook.Data);
			Assert.Equal("foo", webhook.Data["Key"].Value<string>());
			Assert.Equal("bar", webhook.Data["Value"].Value<string>());
		}

		[Fact]
		public async Task ReceiveWebhookFromHttpReceiver() {
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
			var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://callback.deveel.com");
			requestMessage.Content = new StringContent(json.ToString(Newtonsoft.Json.Formatting.None), Encoding.UTF8, "application/json");

			var received = await httpReceiver.ReceiveAsync(requestMessage, default);

			Assert.NotNull(received);
			Assert.Equal(webhook.EventId, received.EventId);
			Assert.Equal(webhook.EventType, received.EventType);
			Assert.NotNull(webhook.Data);
			Assert.Equal("foo", webhook.Data["Key"].Value<string>());
			Assert.Equal("bar", webhook.Data["Value"].Value<string>());
		}

		[Theory]
		[InlineData(WebhookSignatureLocation.Header)]
		[InlineData(WebhookSignatureLocation.QueryString)]
		public async Task ReceiveSignedWebhookFromHttpRequest(WebhookSignatureLocation signatureLocation) {
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
				ValidateSignature = true,
				SignatureLocation = signatureLocation
			};

			var json = JObject.FromObject(webhook);
			var jsonString = json.ToString(Newtonsoft.Json.Formatting.None);
			var signature = new Sha256WebhookSigner().Sign(jsonString, secret);

			var requestUri = new UriBuilder("https://callback.deveel.com");

			if (signatureLocation == WebhookSignatureLocation.QueryString)
				requestUri.Query = $"?sig_alg=sha256&webhook-signature={signature}";

			var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri.Uri);

			if (signatureLocation == WebhookSignatureLocation.Header) {
				requestMessage.Headers.TryAddWithoutValidation(options.SignatureHeaderName, $"sha256={signature}");
			}

			requestMessage.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

			var received = await requestMessage.GetWebhookAsync<WebhookPayload>(options);

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
