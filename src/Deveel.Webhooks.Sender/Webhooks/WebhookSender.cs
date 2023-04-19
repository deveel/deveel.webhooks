using System.Net;
using System.Reflection.Metadata;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Polly;
using Polly.Retry;

namespace Deveel.Webhooks {
	public class WebhookSender<TWebhook> : IWebhookSender<TWebhook>, IDisposable where TWebhook : class {
		private readonly IHttpClientFactory httpClientFactory;
		private readonly bool disposeClient;
		private readonly HttpClient? httpClient;
		private bool disposed = false;

		private readonly IWebhookSignerProvider<TWebhook>? signerProvider;
		private readonly IWebhookJsonSerializer<TWebhook>? jsonSerializer;

		public WebhookSender(IOptionsSnapshot<WebhookSenderOptions> options, 
			IHttpClientFactory httpClientFactory, 
			IWebhookJsonSerializer<TWebhook>? jsonSerializer = null,
			IWebhookSignerProvider<TWebhook>? signerProvider = null) 
			: this(options.Get(typeof(TWebhook).Name), httpClientFactory) {
			this.jsonSerializer = jsonSerializer;
			this.signerProvider = signerProvider;
		}

		protected WebhookSender(WebhookSenderOptions options, IHttpClientFactory httpClientFactory) {
			if (options is null) throw new ArgumentNullException(nameof(options));

			SenderOptions = options;
			this.httpClientFactory = httpClientFactory;

			if (httpClientFactory == null) {
				httpClient = new HttpClient();
				disposeClient = true;
			}
		}

		protected WebhookSender(WebhookSenderOptions options, HttpClient httpClient) {
			if (options is null) 
				throw new ArgumentNullException(nameof(options));

			if (httpClient is null) 
				throw new ArgumentNullException(nameof(httpClient));

			SenderOptions = options;
			this.httpClient = httpClient;
			disposeClient = false;
		}

		protected WebhookSenderOptions SenderOptions { get; }

		protected void ThrowIfDisposed() {
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);
		}

		protected HttpClient CreateClient() {
			ThrowIfDisposed();

			if (httpClientFactory != null) {
				if (String.IsNullOrWhiteSpace(SenderOptions.HttpClientName))
					return httpClientFactory.CreateClient();

				return httpClientFactory.CreateClient(SenderOptions.HttpClientName);
			}
			if (httpClient != null) {
				return httpClient;
			}

			throw new InvalidOperationException("No HTTP client factory or client was set");
		}

		protected virtual IWebhookSigner? GetSigner(string algorithm) {
			ThrowIfDisposed();

			return signerProvider?.GetSigner(algorithm);
		}

		protected virtual async Task<string> SerializeAsync(TWebhook webhook, CancellationToken cancellationToken) {
			if (jsonSerializer == null)
				throw new NotSupportedException("No JSON serializer was set");

			using var stream = new MemoryStream();
			await jsonSerializer.SerializeWebhookAsync(stream, webhook, cancellationToken);
			stream.Position = 0;

			using var reader = new StreamReader(stream);
			return await reader.ReadToEndAsync();
		}

		protected virtual void SignWebhookRequest(HttpRequestMessage request, string jsonBody, string secret) {
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			if (string.IsNullOrWhiteSpace(jsonBody))
				throw new ArgumentNullException(nameof(jsonBody));

			if (SenderOptions.Signature == null ||
				String.IsNullOrWhiteSpace(SenderOptions.Signature.Algorithm))
				return;

			var signer = GetSigner(SenderOptions.Signature.Algorithm);

			if (signer == null)
				throw new InvalidOperationException($"No signature provider found for the algorithm '{SenderOptions.Signature.Algorithm}' configured for the instance");

			var signature = signer.SignWebhook(jsonBody, secret);

			if (SenderOptions.Signature.Location == WebhookSignatureLocation.Header) {
				if (String.IsNullOrWhiteSpace(SenderOptions.Signature.HeaderName))
					throw new WebhookSenderException("The header name for the signature is not set");

				// request.Headers.Add(configuration.DeliveryOptions.SignatureHeaderName, $"{provider.Algorithm}={signature}");
				request.Headers.Add(SenderOptions.Signature.HeaderName, signature);
			} else if (SenderOptions.Signature.Location == WebhookSignatureLocation.QueryString) {
				var originalUrl = new UriBuilder(request.RequestUri);
				var queryString = new StringBuilder(originalUrl.Query);
				if (queryString.Length > 0)
					queryString.Append('&');

				queryString.Append(SenderOptions.Signature.QueryParameter);
				queryString.Append('=');
				queryString.Append(signature);
				if (!String.IsNullOrWhiteSpace(SenderOptions.Signature.AlgorithmQueryParameter)) {
					queryString.Append('&');
					// queryString.Append($"sig_alg={provider.Algorithm}");
					queryString.Append(SenderOptions.Signature.AlgorithmQueryParameter);
					queryString.Append('=');
					queryString.Append(signer.Algorithms[0]);
				}

				originalUrl.Query = queryString.ToString();
				request.RequestUri = originalUrl.Uri;
			}
		}

		protected virtual void AddHeaders(HttpRequestMessage request, IDictionary<string, string>? headers) {
			if (headers != null) {
				foreach (var header in headers) {
					if (request.Headers.TryAddWithoutValidation(header.Key, header.Value))
						continue;

					if (request.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value) ?? false)
						continue;

					throw new WebhookSenderException($"Invalid header in the webhook: {header.Key}");
				}
			}
		}

		protected HttpRequestMessage CreateRequest(WebhookDestination destination) {
			var request = new HttpRequestMessage(HttpMethod.Post, destination.Url);

			AddHeaders(request, destination.Headers);

			return request;
		}

		private WebhookDestination CreateDestination(WebhookDestination source) {
			var result = new WebhookDestination(source.Url) {
				Secret = source.Secret,
				Sign = source.Sign,
				Headers = new Dictionary<string, string>(),
				Verify = source.Verify ?? SenderOptions.VerifyReceivers
			};

			if (SenderOptions.DefaultHeaders != null) {
				foreach (var header in SenderOptions.DefaultHeaders) {
					result.Headers.Add(header.Key, header.Value);
				}
			}

			if (source.Headers != null) {
				foreach (var header in source.Headers) {
					result.Headers[header.Key] = header.Value;
				}
			}

			if (source.Signature == null) {
				result.Signature = SenderOptions.Signature;
			} else {
				result.Signature = new WebhookSenderSignatureOptions {
					Algorithm = source.Signature.Algorithm ?? SenderOptions.Signature?.Algorithm,
					HeaderName = source.Signature.HeaderName ?? SenderOptions.Signature?.HeaderName,
					Location = source.Signature.Location ?? SenderOptions.Signature?.Location,
					QueryParameter = source.Signature.QueryParameter ?? SenderOptions.Signature?.QueryParameter,
					AlgorithmQueryParameter = source.Signature.AlgorithmQueryParameter ?? SenderOptions.Signature?.AlgorithmQueryParameter
				};
			}

			if (source.Retry == null) {
				result.Retry = SenderOptions.Retry;
			} else {
				result.Retry = new WebhookRetryOptions {
					MaxRetries = source.Retry.MaxRetries ?? SenderOptions.Retry?.MaxRetries,
					MaxDelay = source.Retry.MaxDelay ?? SenderOptions.Retry?.MaxDelay,
					TimeOut = source.Retry.TimeOut ?? SenderOptions.Retry?.TimeOut
				};
			}

			return result;
		}

		private async Task<HttpRequestMessage> CreateRequestAsync(WebhookDestination destination, TWebhook webhook, CancellationToken cancellationToken) {
			try {
				var request = CreateRequest(destination);

				var jsonBody = await SerializeAsync(webhook, cancellationToken);

				if (destination.Sign ?? false && !String.IsNullOrWhiteSpace(destination.Secret)) {
					SignWebhookRequest(request, jsonBody, destination.Secret!);
				}

				request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

				return request;
			} catch (WebhookSenderException) {

				throw;
			} catch(Exception ex) {
				throw new WebhookSenderException("An error occurred while creating the request", ex);
			}
		}

		private AsyncRetryPolicy CreatePolicy(int? retryCount) {
			var maxRetries = retryCount ?? SenderOptions.Retry?.MaxRetries ?? 1;
			// generate the waiting times for the retries
			var waitTimes = new TimeSpan[maxRetries];
			for (var i = 0; i < maxRetries; i++) {
				waitTimes[i] = TimeSpan.FromSeconds(i * (i + 1));
			}

			// the retry policy
			return Policy.Handle<TaskCanceledException>()
				.Or<TimeoutException>()
				.Or<HttpRequestException>()
				.WaitAndRetryAsync(waitTimes);
		}

		private async Task SendRequestAsync(HttpRequestMessage request, WebhookDeliveryResult<TWebhook> result, CancellationToken cancellationToken) {
			var attempt = result.StartAttempt();

			HttpResponseMessage? response = null;

			try {
				response = await SendRequestAsync(request, cancellationToken);

				if (response.StatusCode == HttpStatusCode.RequestTimeout) {
					attempt.TimeOut();
				} else {
					attempt.Complete((int)response.StatusCode, response.ReasonPhrase);
				}

				response.EnsureSuccessStatusCode();
			} catch (TimeoutException) {
				// logger.LogWarning("The delivery attempt {AttemptNumber} timed-out", attempt.Number);

				attempt.TimeOut();
				throw;
			} catch (HttpRequestException ex) {
				// logger.LogWarning(ex, "The delivery attempt {AttemptNumber} failed", attempt.Number);

				if (response != null) {
					attempt.Complete((int)response.StatusCode, response.ReasonPhrase);
				} else {
					attempt.LocalFail($"Remote error: {ex.Message}");
				}

				throw;
			} catch (WebhookSenderException ex) {
				// logger.LogError(ex, "The delivery attempt {AttemptNumber} caused an exception", attempt.Number);

				attempt.LocalFail($"Local error: {ex.Message}");
				throw;
			} catch (Exception ex) {
				// logger.LogError(ex, "The delivery attempt {AttemptNumber} caused an exception", attempt.Number);

				attempt.LocalFail($"Local error: {ex.Message}");
				throw new WebhookSenderException("Could not send the webhook", ex);
			}
		}

		protected virtual Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
			// we don't dispose the client, because it might be a singleton...
			// 1. if the client is coming from the IHttpClientFactory, it will be disposed by the factory
			// 2. if the client was set at the constructor, it will be disposed by the caller
			// 3. if the client was created by the sender, it will be disposed when the sender is disposed
			var client = CreateClient();

			return client.SendAsync(request, cancellationToken);
		}

		public async Task<WebhookDeliveryResult<TWebhook>> SendAsync(WebhookDestination receiver, TWebhook webhook, CancellationToken cancellationToken) {
			try {
				var destination = CreateDestination(receiver);
				var result = new WebhookDeliveryResult<TWebhook>(destination, webhook);

				var policy = CreatePolicy(destination.Retry?.MaxRetries ?? 1);

				var request = await CreateRequestAsync(destination, webhook, cancellationToken);

				await policy.ExecuteAndCaptureAsync(token => SendRequestAsync(request, result, token), cancellationToken);

				return result;
			} catch (WebhookSenderException) {

				throw;
			} catch(Exception ex) {
				throw new WebhookSenderException("An error occurred while sending the webhook", ex);
			}
		}

		protected virtual void Dispose(bool disposing) {
			if (!disposed) {
				if (disposing && disposeClient) {
					httpClient?.Dispose();
				}

				disposed = true;
			}
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
