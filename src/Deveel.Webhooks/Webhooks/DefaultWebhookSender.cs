using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Polly;

namespace Deveel.Webhooks {
	public class DefaultWebhookSender : IWebhookSender, IDisposable {
		private readonly HttpClient httpClient;
		private readonly bool disposeClient;
		private readonly WebhookDeliveryOptions deliveryOptions;
		private readonly IEnumerable<IWebhookSignatureProvider> signatureProviders;
		private readonly ILogger logger;

		private DefaultWebhookSender(HttpClient httpClient, bool disposeClient, WebhookDeliveryOptions deliveryOptions, IEnumerable<IWebhookSignatureProvider> signatureProviders, ILogger logger) {
			if (deliveryOptions == null) {
				logger.LogWarning("Delivery options was not set: resetting to default");
				deliveryOptions = new WebhookDeliveryOptions();
			}

			if (httpClient == null) {
				logger.LogWarning("HTTP Client was not set: setting to default");

				httpClient = new HttpClient();
				disposeClient = true;
			}

			this.httpClient = httpClient;
			this.deliveryOptions = deliveryOptions;
			this.disposeClient = disposeClient;
			this.logger = logger;
			this.signatureProviders = signatureProviders;
		}

		public DefaultWebhookSender(HttpClient httpClient, IOptions<WebhookDeliveryOptions> deliveryOptions, IEnumerable<IWebhookSignatureProvider> signatureProviders, ILogger<DefaultWebhookSender> logger)
			: this(httpClient, deliveryOptions?.Value, signatureProviders, logger) {
		}

		public DefaultWebhookSender(HttpClient httpClient, IOptions<WebhookDeliveryOptions> deliveryOptions, IEnumerable<IWebhookSignatureProvider> signatureProviders)
			: this(httpClient, deliveryOptions?.Value, signatureProviders, NullLogger<DefaultWebhookSender>.Instance) {
		}

		public DefaultWebhookSender(IOptions<WebhookDeliveryOptions> deliveryOptions, IEnumerable<IWebhookSignatureProvider> signatureProviders, ILogger<DefaultWebhookSender> logger)
			: this(new HttpClient(), true, deliveryOptions?.Value, signatureProviders, logger) {
		}

		public DefaultWebhookSender(IOptions<WebhookDeliveryOptions> deliveryOptions, IEnumerable<IWebhookSignatureProvider> signatureProviders)
			: this(new HttpClient(), deliveryOptions?.Value, signatureProviders, NullLogger<DefaultWebhookSender>.Instance) {
		}

		public DefaultWebhookSender(IHttpClientFactory httpClientFactory, IOptions<WebhookDeliveryOptions> deliveryOptions, IEnumerable<IWebhookSignatureProvider> signatureProviders, ILogger<DefaultWebhookSender> logger)
			: this(httpClientFactory, deliveryOptions?.Value, signatureProviders, logger) {
		}

		public DefaultWebhookSender(IHttpClientFactory httpClientFactory, IOptions<WebhookDeliveryOptions> deliveryOptions, IEnumerable<IWebhookSignatureProvider> signatureProviders)
			: this(httpClientFactory, deliveryOptions?.Value, signatureProviders, NullLogger<DefaultWebhookSender>.Instance) {
		}


		public DefaultWebhookSender(WebhookDeliveryOptions deliveryOptions, IEnumerable<IWebhookSignatureProvider> signatureProviders, ILogger<DefaultWebhookSender> logger)
			: this(new HttpClient(), true, deliveryOptions, signatureProviders, logger) {
		}

		public DefaultWebhookSender(WebhookDeliveryOptions deliveryOptions, IEnumerable<IWebhookSignatureProvider> signatureProviders)
			: this(new HttpClient(), deliveryOptions, signatureProviders, NullLogger<DefaultWebhookSender>.Instance) {
		}

		public DefaultWebhookSender(HttpClient httpClient, WebhookDeliveryOptions deliveryOptions, IEnumerable<IWebhookSignatureProvider> signatureProviders, ILogger<DefaultWebhookSender> logger)
			: this(httpClient, false, deliveryOptions, signatureProviders, logger) {
		}

		public DefaultWebhookSender(HttpClient httpClient, WebhookDeliveryOptions deliveryOptions, IEnumerable<IWebhookSignatureProvider> signatureProviders)
			: this(httpClient, deliveryOptions, signatureProviders, NullLogger<DefaultWebhookSender>.Instance) {
		}


		public DefaultWebhookSender(IHttpClientFactory httpClientFactory, WebhookDeliveryOptions deliveryOptions, IEnumerable<IWebhookSignatureProvider> signatureProviders, ILogger<DefaultWebhookSender> logger)
			: this(httpClientFactory.CreateClient(), false, deliveryOptions, signatureProviders, logger) {
		}

		public DefaultWebhookSender(IHttpClientFactory httpClientFactory, WebhookDeliveryOptions deliveryOptions, IEnumerable<IWebhookSignatureProvider> signatureProviders)
			: this(httpClientFactory, deliveryOptions, signatureProviders, NullLogger<DefaultWebhookSender>.Instance) {
		}

		protected virtual void SignWebhookRequest(HttpRequestMessage request, string serializedBody, string secret) {
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			if (string.IsNullOrWhiteSpace(serializedBody))
				throw new ArgumentNullException(nameof(serializedBody));

			var provider = signatureProviders?
				.FirstOrDefault(x => x.Algorithm == deliveryOptions.SignatureAlgorithm);

			if (provider == null)
				throw new InvalidOperationException($"No signature provider found for the algorithm '{deliveryOptions.SignatureAlgorithm}' configured for the instance");

			var signature = provider.Sign(serializedBody, secret);

			if (deliveryOptions.SignatureLocation == WebhookSignatureLocation.Header) {
				request.Headers.Add(deliveryOptions.HeaderName, $"{provider.Algorithm}={signature}");
			} else if (deliveryOptions.SignatureLocation == WebhookSignatureLocation.QueryString) {
				var originalUrl = new UriBuilder(request.RequestUri);
				var queryString = new StringBuilder(originalUrl.Query);
				if (queryString.Length > 0)
					queryString.Append('&');

				queryString.Append(deliveryOptions.QueryStringKey);
				queryString.Append('=');
				queryString.Append(signature);
				queryString.Append('&');
				queryString.Append($"sig_alg={provider.Algorithm}");

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
			var payload = new WebhookPayload();

			var fields = deliveryOptions?.IncludeFields ?? WebhookFields.All;

			if ((fields & WebhookFields.EventId) != 0)
				payload.EventId = webhook.Id;
			if ((fields & WebhookFields.EventName) != 0)
				payload.EventType = webhook.EventType;
			if ((fields & WebhookFields.TimeStamp) != 0)
				payload.TimeStamp = webhook.TimeStamp;
			if ((fields & WebhookFields.Name) != 0)
				payload.WebhookName = webhook.Name;

			payload.Data = JObject.FromObject(webhook.Data);

			return Task.FromResult(payload);
		}

		public virtual async Task<string> GetSerializedBodyAsync(IWebhook webhook) {
			var payload = await GetWebhookPayloadAsync(webhook);

			var serializedBody = deliveryOptions.JsonSerializerSettings != null
				? JsonConvert.SerializeObject(payload, deliveryOptions.JsonSerializerSettings)
				: JsonConvert.SerializeObject(payload);

			return serializedBody;
		}

		private async Task<HttpRequestMessage> BuildRequestAsync(IWebhook webhook) {
			try {
				var request = CreateWebhookRequestMessage(webhook);

				var serializedBody = await GetSerializedBodyAsync(webhook);

				request.Content = new StringContent(serializedBody, Encoding.UTF8, "application/json");

				if (deliveryOptions.SignWebhooks && !string.IsNullOrWhiteSpace(webhook.Secret))
					SignWebhookRequest(request, serializedBody, webhook.Secret);

				AddAdditionalHeaders(request, webhook);

				return request;
			} catch (Exception ex) {
				logger.LogError(ex, "Error while request object for webhook of type {EventType}", webhook.EventType);
				throw;
			}
		}

		/// <inheritdoc />
		public async Task<WebhookDeliveryResult> SendAsync(IWebhook webhook, CancellationToken cancellationToken) {
			try {
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
			} catch (Exception ex) {
				logger.LogError(ex, "Error while sending a webhook of type {EventType}", webhook.EventType);
				throw;
			}
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
