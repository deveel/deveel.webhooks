using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Polly;

namespace Deveel.Webhooks {
	public class DefaultWebhookSender : IWebhookSender, IDisposable {
		private readonly HttpClient httpClient;
		private readonly bool disposeClient;
		private readonly WebhookDeliveryOptions deliveryOptions;

		private DefaultWebhookSender(HttpClient httpClient, WebhookDeliveryOptions deliveryOptions, bool disposeClient) {
			this.httpClient = httpClient;
			this.deliveryOptions = deliveryOptions;
			this.disposeClient = disposeClient;
		}

		public DefaultWebhookSender(WebhookDeliveryOptions deliveryOptions)
			: this(new HttpClient(), deliveryOptions, true) {
		}

		public DefaultWebhookSender(HttpClient httpClient, WebhookDeliveryOptions deliveryOptions)
			: this(httpClient, deliveryOptions, false) {
		}

		public DefaultWebhookSender(HttpClient httpClient, IOptions<WebhookDeliveryOptions> deliveryOptions)
			: this(httpClient, deliveryOptions?.Value) {
		}

		public DefaultWebhookSender(IOptions<WebhookDeliveryOptions> deliveryOptions)
			: this(new HttpClient(), deliveryOptions?.Value, true) {
		}

		protected virtual void SignWebhookRequest(HttpRequestMessage request, string serializedBody, string secret) {
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			if (string.IsNullOrWhiteSpace(serializedBody))
				throw new ArgumentNullException(nameof(serializedBody));

			var secretBytes = Encoding.UTF8.GetBytes(secret);

			string signature;

			using (var hasher = new HMACSHA256(secretBytes)) {
				var data = Encoding.UTF8.GetBytes(serializedBody);
				var sha256 = hasher.ComputeHash(data);

				signature = BitConverter.ToString(sha256);
			}

			if (deliveryOptions.SignatureLocation == WebhookSignatureLocation.Header) {
				request.Headers.Add(deliveryOptions.HeaderName, $"sha256={signature}");
			} else if (deliveryOptions.SignatureLocation == WebhookSignatureLocation.QueryString) {
				var originalUrl = new UriBuilder(request.RequestUri);
				var queryString = new StringBuilder(originalUrl.Query);
				if (queryString.Length > 0)
					queryString.Append('&');

				queryString.Append(deliveryOptions.QueryStringKey);
				queryString.Append('=');
				queryString.Append(signature);
				queryString.Append('&');
				queryString.Append("sig_alg=sha256");

				originalUrl.Query = queryString.ToString();
				request.RequestUri = originalUrl.Uri;
			}
		}

		protected virtual void AddAdditionalHeaders(HttpRequestMessage request, IWebhook webhook) {
			if (webhook.Headers != null) {
				foreach (var header in webhook.Headers) {
					if (request.Headers.TryAddWithoutValidation(header.Key, header.Value))
						continue;
					if (request.Content.Headers.TryAddWithoutValidation(header.Key, header.Value))
						continue;

					throw new InvalidOperationException($"Invalid header in the webhook: {header.Key}");
				}
			}
		}

		protected virtual HttpRequestMessage CreateWebhookRequestMessage(IWebhook webhook) {
			return new HttpRequestMessage(HttpMethod.Post, webhook.DestinationUrl);
		}

		public virtual Task<WebhookPayload> GetWebhookPayloadAsync(IWebhook webhook) {
			return Task.FromResult(new WebhookPayload {
				WebhookName = webhook.Name,
				EventType = webhook.EventType,
				EventId = webhook.Id,
				TimeStamp = webhook.TimeStamp,
				Data = GetExtensionData(webhook.Data)
			});
		}

		private JObject GetExtensionData(object data) {
			return JObject.FromObject(data);
		}

		public virtual async Task<string> GetSerializedBodyAsync(IWebhook webhook) {
			var payload = await GetWebhookPayloadAsync(webhook);

			var serializedBody = deliveryOptions.JsonSerializerSettings != null
				? JsonConvert.SerializeObject(payload, deliveryOptions.JsonSerializerSettings)
				: JsonConvert.SerializeObject(payload);

			return serializedBody;
		}

		private async Task<HttpRequestMessage> BuildRequestAsync(IWebhook webhook) {
			var request = CreateWebhookRequestMessage(webhook);

			var serializedBody = await GetSerializedBodyAsync(webhook);

			request.Content = new StringContent(serializedBody, Encoding.UTF8, "application/json");

			if (deliveryOptions.SignWebhooks && !string.IsNullOrWhiteSpace(webhook.Secret))
				SignWebhookRequest(request, serializedBody, webhook.Secret);

			AddAdditionalHeaders(request, webhook);

			return request;
		}

		/// <inheritdoc />
		public async Task<WebhookDeliveryResult> SendAsync(IWebhook webhook, CancellationToken cancellationToken) {
			var result = new WebhookDeliveryResult(webhook);

			var waitTimeCount = deliveryOptions.MaxAttemptCount - 1 <= 0 ? 1 : deliveryOptions.MaxAttemptCount - 1;
			var waitTimes = new TimeSpan[waitTimeCount];
			for (var i = 0; i < waitTimeCount; i++) {
				waitTimes[i] = TimeSpan.FromSeconds(i * (i + 1));
			}

			var policy = Policy.Handle<TaskCanceledException>()
				.Or<TimeoutException>()
				.Or<HttpRequestException>()
				.WaitAndRetryAsync(waitTimes);

			var captured = await policy.ExecuteAndCaptureAsync(async () => {
				var attempt = new WebhookDeliveryAttempt();
				

				try {
					var request = await BuildRequestAsync(webhook);
					var response = await SendHttpRequest(request);

					if (response.StatusCode == HttpStatusCode.RequestTimeout) {
						attempt.Timeout();
					} else {
						attempt.Finish((int)response.StatusCode, response.ReasonPhrase);
					}

					response.EnsureSuccessStatusCode();
				} catch (TimeoutException) {
					attempt.Timeout();
					throw;
				} finally {
					result.AddAttempt(attempt);
				}
			});

			return result;
		}

		protected virtual Task<HttpResponseMessage> SendHttpRequest(HttpRequestMessage request) {
			return httpClient.SendAsync(request);
		}

		public void Dispose() {
			if (disposeClient)
				httpClient?.Dispose();
		}
	}
}
