// Copyright 2022 Deveel
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

using System;
using System.Net;
using System.Net.Http;
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
	public class WebhookSender : IWebhookSender, IDisposable {
		private readonly HttpClient httpClient;
		private readonly bool disposeClient;
		private readonly WebhookDeliveryOptions deliveryOptions;
		private readonly IWebhookSignerSelector signerSelector;
		private readonly ILogger logger;

		#region .ctor

		private WebhookSender(HttpClient httpClient, bool disposeClient,
			WebhookDeliveryOptions deliveryOptions,
			IWebhookSignerSelector signerSelector,
			ILogger logger) {
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
			this.signerSelector = signerSelector;
		}

		public WebhookSender(HttpClient httpClient,
			IOptions<WebhookDeliveryOptions> deliveryOptions,
			IWebhookSignerSelector signerSelector,
			ILogger<WebhookSender> logger)
			: this(httpClient, deliveryOptions?.Value, signerSelector, logger) {
		}

		public WebhookSender(HttpClient httpClient,
			IOptions<WebhookDeliveryOptions> deliveryOptions,
			ILogger<WebhookSender> logger)
			: this(httpClient, deliveryOptions, new SingletonWebhookSignerSelector(new Sha256WebhookSigner()), logger) {
		}

		public WebhookSender(HttpClient httpClient,
			IOptions<WebhookDeliveryOptions> deliveryOptions,
			IWebhookSignerSelector signerSelector)
			: this(httpClient, deliveryOptions?.Value, signerSelector, NullLogger<WebhookSender>.Instance) {
		}

		public WebhookSender(HttpClient httpClient,
			IOptions<WebhookDeliveryOptions> deliveryOptions)
			: this(httpClient, deliveryOptions?.Value, new SingletonWebhookSignerSelector(new Sha256WebhookSigner()), NullLogger<WebhookSender>.Instance) {
		}

		public WebhookSender(IOptions<WebhookDeliveryOptions> deliveryOptions,
			IWebhookSignerSelector signerSelector,
			ILogger<WebhookSender> logger)
			: this(new HttpClient(), true, deliveryOptions?.Value, signerSelector, logger) {
		}

		public WebhookSender(IOptions<WebhookDeliveryOptions> deliveryOptions, ILogger<WebhookSender> logger)
			: this(deliveryOptions, new SingletonWebhookSignerSelector(new Sha256WebhookSigner()), logger) {
		}

		public WebhookSender(IOptions<WebhookDeliveryOptions> deliveryOptions, IWebhookSignerSelector signerSelector)
			: this(new HttpClient(), deliveryOptions?.Value, signerSelector, NullLogger<WebhookSender>.Instance) {
		}

		public WebhookSender(IOptions<WebhookDeliveryOptions> deliveryOptions)
			: this(deliveryOptions, new SingletonWebhookSignerSelector(new Sha256WebhookSigner())) {
		}

		public WebhookSender(IHttpClientFactory httpClientFactory,
			IOptions<WebhookDeliveryOptions> deliveryOptions,
			IWebhookSignerSelector signerSelector,
			ILogger<WebhookSender> logger)
			: this(httpClientFactory, deliveryOptions?.Value, signerSelector, logger) {
		}

		public WebhookSender(IHttpClientFactory httpClientFactory,
			IOptions<WebhookDeliveryOptions> deliveryOptions,
			ILogger<WebhookSender> logger)
			: this(httpClientFactory, deliveryOptions, new SingletonWebhookSignerSelector(new Sha256WebhookSigner()), logger) {
		}

		public WebhookSender(IHttpClientFactory httpClientFactory,
			IOptions<WebhookDeliveryOptions> deliveryOptions,
			IWebhookSignerSelector signerSelector)
			: this(httpClientFactory, deliveryOptions?.Value, signerSelector, NullLogger<WebhookSender>.Instance) {
		}

		public WebhookSender(IHttpClientFactory httpClientFactory, IOptions<WebhookDeliveryOptions> deliveryOptions)
			: this(httpClientFactory, deliveryOptions, new SingletonWebhookSignerSelector(new Sha256WebhookSigner())) {
		}

		public WebhookSender(WebhookDeliveryOptions deliveryOptions, IWebhookSignerSelector signerSelector, ILogger<WebhookSender> logger)
			: this(new HttpClient(), true, deliveryOptions, signerSelector, logger) {
		}

		public WebhookSender(WebhookDeliveryOptions deliveryOptions, ILogger<WebhookSender> logger)
			: this(deliveryOptions, new SingletonWebhookSignerSelector(new Sha256WebhookSigner()), logger) {
		}

		public WebhookSender(WebhookDeliveryOptions deliveryOptions, IWebhookSignerSelector signerSelector)
			: this(new HttpClient(), deliveryOptions, signerSelector, NullLogger<WebhookSender>.Instance) {
		}

		public WebhookSender(WebhookDeliveryOptions deliveryOptions)
			: this(deliveryOptions, new SingletonWebhookSignerSelector(new Sha256WebhookSigner())) {
		}

		public WebhookSender(HttpClient httpClient, WebhookDeliveryOptions deliveryOptions, IWebhookSignerSelector signerSelector, ILogger<WebhookSender> logger)
			: this(httpClient, false, deliveryOptions, signerSelector, logger) {
		}

		public WebhookSender(HttpClient httpClient, WebhookDeliveryOptions deliveryOptions, ILogger<WebhookSender> logger)
			: this(httpClient, deliveryOptions, new SingletonWebhookSignerSelector(new Sha256WebhookSigner()), logger) {
		}

		public WebhookSender(HttpClient httpClient, WebhookDeliveryOptions deliveryOptions, IWebhookSignerSelector signerSelector)
			: this(httpClient, deliveryOptions, signerSelector, NullLogger<WebhookSender>.Instance) {
		}

		public WebhookSender(HttpClient httpClient, WebhookDeliveryOptions deliveryOptions)
			: this(httpClient, deliveryOptions, new SingletonWebhookSignerSelector(new Sha256WebhookSigner())) {
		}

		public WebhookSender(IHttpClientFactory httpClientFactory, WebhookDeliveryOptions deliveryOptions, IWebhookSignerSelector signerSelector, ILogger<WebhookSender> logger)
			: this(httpClientFactory.CreateClient(), false, deliveryOptions, signerSelector, logger) {
		}

		public WebhookSender(IHttpClientFactory httpClientFactory, WebhookDeliveryOptions deliveryOptions, ILogger<WebhookSender> logger)
			: this(httpClientFactory, deliveryOptions, new SingletonWebhookSignerSelector(new Sha256WebhookSigner()), logger) {
		}


		public WebhookSender(IHttpClientFactory httpClientFactory, WebhookDeliveryOptions deliveryOptions, IWebhookSignerSelector signerSelector)
			: this(httpClientFactory, deliveryOptions, signerSelector, NullLogger<WebhookSender>.Instance) {
		}

		public WebhookSender(IHttpClientFactory httpClientFactory, WebhookDeliveryOptions deliveryOptions)
			: this(httpClientFactory, deliveryOptions, new SingletonWebhookSignerSelector(new Sha256WebhookSigner())) {
		}

		#endregion

		protected virtual void SignWebhookRequest(HttpRequestMessage request, string serializedBody, string secret) {
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			if (string.IsNullOrWhiteSpace(serializedBody))
				throw new ArgumentNullException(nameof(serializedBody));

			var provider = signerSelector?.GetSigner(deliveryOptions.SignatureAlgorithm);

			if (provider == null)
				throw new InvalidOperationException($"No signature provider found for the algorithm '{deliveryOptions.SignatureAlgorithm}' configured for the instance");

			var signature = provider.Sign(serializedBody, secret);

			if (deliveryOptions.SignatureLocation == WebhookSignatureLocation.Header) {
				request.Headers.Add(deliveryOptions.SignatureHeaderName, $"{provider.Algorithm}={signature}");
			} else if (deliveryOptions.SignatureLocation == WebhookSignatureLocation.QueryString) {
				var originalUrl = new UriBuilder(request.RequestUri);
				var queryString = new StringBuilder(originalUrl.Query);
				if (queryString.Length > 0)
					queryString.Append('&');

				queryString.Append(deliveryOptions.SignatureQueryStringKey);
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

			// first normalize the string to the JSON serialization settings
			var dataJson = JsonConvert.SerializeObject(webhook.Data, deliveryOptions.JsonSerializerSettings);

			payload.Data = JObject.Parse(dataJson);

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
