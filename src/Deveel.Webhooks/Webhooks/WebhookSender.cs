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
	/// <summary>
	/// A default implementation of the <see cref="IWebhookSender"/> service
	/// </summary>
	/// <seealso cref="IWebhookSender"/>
	public class WebhookSender<TWebhook> : IWebhookSender<TWebhook>, IDisposable where TWebhook : class, IWebhook {
		private readonly HttpClient httpClient;
		private readonly bool disposeClient;
		private readonly IWebhookServiceConfiguration configuration;
		private readonly ILogger logger;

		#region .ctor

		private WebhookSender(HttpClient httpClient, bool disposeClient,
			IWebhookServiceConfiguration configuration,
			ILogger logger) {
			if (httpClient == null) {
				logger.LogWarning("HTTP Client was not set: setting to default");

				httpClient = new HttpClient();
				disposeClient = true;
			}

			this.httpClient = httpClient;
			this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			this.disposeClient = disposeClient;
			this.logger = logger;
		}

		public WebhookSender(HttpClient httpClient,
			IWebhookServiceConfiguration configuration,
			ILogger<WebhookSender<TWebhook>> logger)
			: this(httpClient, false, configuration, logger) {
		}

		public WebhookSender(HttpClient httpClient, IWebhookServiceConfiguration configuration)
			: this(httpClient, configuration, NullLogger<WebhookSender<TWebhook>>.Instance) {
		}

		public WebhookSender(IWebhookServiceConfiguration configuration, ILogger<WebhookSender<TWebhook>> logger)
			: this(new HttpClient(), true, configuration, logger) {
		}

		public WebhookSender(IWebhookServiceConfiguration configuration)
			: this(new HttpClient(), configuration, NullLogger<WebhookSender<TWebhook>>.Instance) {
		}

		public WebhookSender(IHttpClientFactory httpClientFactory, IWebhookServiceConfiguration configuration, ILogger<WebhookSender<TWebhook>> logger)
			: this(httpClientFactory.CreateClient(), false, configuration, logger) {
		}

		public WebhookSender(IHttpClientFactory httpClientFactory, IWebhookServiceConfiguration configuration)
			: this(httpClientFactory, configuration, NullLogger<WebhookSender<TWebhook>>.Instance) {
		}

		#endregion

		protected virtual void SignWebhookRequest(HttpRequestMessage request, string serializedBody, string secret) {
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			if (string.IsNullOrWhiteSpace(serializedBody))
				throw new ArgumentNullException(nameof(serializedBody));

			// TODO: get the algorithm from the subscription / webhook
			var provider = configuration.Signers.DefaultSigner;

			if (provider == null)
				throw new InvalidOperationException($"No signature provider found for the algorithm '{configuration.DeliveryOptions.SignatureAlgorithm}' configured for the instance");

			var signature = provider.SignWebhook(serializedBody, secret);

			if (configuration.DeliveryOptions.SignatureLocation == WebhookSignatureLocation.Header) {
				// request.Headers.Add(configuration.DeliveryOptions.SignatureHeaderName, $"{provider.Algorithm}={signature}");
				request.Headers.Add(configuration.DeliveryOptions.SignatureHeaderName, signature);
			} else if (configuration.DeliveryOptions.SignatureLocation == WebhookSignatureLocation.QueryString) {
				var originalUrl = new UriBuilder(request.RequestUri);
				var queryString = new StringBuilder(originalUrl.Query);
				if (queryString.Length > 0)
					queryString.Append('&');

				queryString.Append(configuration.DeliveryOptions.SignatureQueryStringKey);
				queryString.Append('=');
				queryString.Append(signature);
				queryString.Append('&');
				// queryString.Append($"sig_alg={provider.Algorithm}");
				queryString.Append($"sig_alg={provider.Algorithms[0]}");

				originalUrl.Query = queryString.ToString();
				request.RequestUri = originalUrl.Uri;
			}
		}

		/// <summary>
		/// Adds additional headers contained in the webhook to the HTTP request
		/// used to notify it
		/// </summary>
		/// <param name="request">The HTTP request to notify the webhook</param>
		/// <param name="webhook">The webhook object to be notified</param>
		/// <exception cref="WebhookException">
		/// Thrown if it was not possible to add one of the webhook additional headers to
		/// the request object 
		/// </exception>
		protected virtual void AddAdditionalHeaders(HttpRequestMessage request, IWebhook webhook) {
			if (webhook.Headers != null) {
				foreach (var header in webhook.Headers) {
					if (request.Headers.TryAddWithoutValidation(header.Key, header.Value))
						continue;
					if (request.Content.Headers.TryAddWithoutValidation(header.Key, header.Value))
						continue;

					throw new WebhookException($"Invalid header in the webhook: {header.Key}");
				}
			}
		}

		protected virtual HttpRequestMessage CreateWebhookRequestMessage(IWebhook webhook) {
			return new HttpRequestMessage(HttpMethod.Post, webhook.DestinationUrl);
		}

		private async Task<HttpRequestMessage> BuildRequestAsync(TWebhook webhook, CancellationToken cancellationToken) {
			try {
				var request = CreateWebhookRequestMessage(webhook);

				var serializer = configuration.Serializers["json"];

				await serializer.WriteAsync(request, webhook, cancellationToken);

				// request.Content = new StringContent(serializedBody, Encoding.UTF8, "application/json");

				if (configuration.DeliveryOptions.SignWebhooks &&
					!string.IsNullOrWhiteSpace(webhook.Secret)) {
					var serializedBody = await serializer.GetAsStringAsync(webhook, cancellationToken);
					SignWebhookRequest(request, serializedBody, webhook.Secret);
				}

				AddAdditionalHeaders(request, webhook);

				return request;
			} catch(WebhookException ex) {
				logger.LogError(ex, "Error while request object for webhook of type {EventType}", webhook.EventType);
				throw;
			} catch (Exception ex) {
				logger.LogError(ex, "Error while request object for webhook of type {EventType}", webhook.EventType);
				throw new WebhookException("An error occurred while building the request", ex);
			}
		}

		/// <inheritdoc />
		public async Task<WebhookDeliveryResult> SendAsync(TWebhook webhook, CancellationToken cancellationToken) {
			try {
				var result = new WebhookDeliveryResult(webhook);

				// generate the waiting times for the retries
				var waitTimeCount = configuration.DeliveryOptions.MaxAttemptCount - 1 <= 0 ? 1 : configuration.DeliveryOptions.MaxAttemptCount - 1;
				var waitTimes = new TimeSpan[waitTimeCount];
				for (var i = 0; i < waitTimeCount; i++) {
					waitTimes[i] = TimeSpan.FromSeconds(i * (i + 1));
				}

				// the retry policy
				var policy = Policy.Handle<TaskCanceledException>()
					.Or<TimeoutException>()
					.Or<HttpRequestException>()
					.WaitAndRetryAsync(waitTimes);

				var captured = await policy.ExecuteAndCaptureAsync(async () => {
					var attempt = result.StartAttempt();

					HttpResponseMessage response = null;

					try {
						var request = await BuildRequestAsync(webhook, cancellationToken);
						response = await SendHttpRequest(request);

						if (response.StatusCode == HttpStatusCode.RequestTimeout) {
							attempt.Timeout();
						} else {
							attempt.Finish((int)response.StatusCode, response.ReasonPhrase);
						}

						response.EnsureSuccessStatusCode();
					} catch (TimeoutException) {
						logger.LogWarning("The delivery attempt {AttemptNumber} timed-out", attempt.Number);

						attempt.Timeout();
						throw;
					} catch (HttpRequestException ex) {
						logger.LogWarning(ex, "The delivery attempt {AttemptNumber} failed", attempt.Number);

						if (response != null) {
							attempt.Finish((int)response.StatusCode, response.ReasonPhrase);
						} else {
							attempt.Finish(null, $"Remote error: {ex.Message}");
						}

						throw;
					} catch (WebhookException ex) {
						logger.LogError(ex, "The delivery attempt {AttemptNumber} caused an exception", attempt.Number);

						attempt.Finish(null, $"Local error: {ex.Message}");
						throw;
					} catch (Exception ex) {
						logger.LogError(ex, "The delivery attempt {AttemptNumber} caused an exception", attempt.Number);

						attempt.Finish(null, $"Local error: {ex.Message}");
						throw new WebhookException("Could not send the webhook", ex);
					}
				});

				return result;
			} catch(WebException ex) {
				logger.LogError(ex, "Error while sending a webhook of type {EventType}", webhook.EventType);
				throw;
			} catch (Exception ex) {
				logger.LogError(ex, "Error while sending a webhook of type {EventType}", webhook.EventType);
				throw new WebhookException("Could not send the webhook", ex);
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
