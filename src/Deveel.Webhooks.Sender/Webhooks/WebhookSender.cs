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

using System.Text;

using Microsoft.Extensions.Options;

using Polly;
using Polly.Timeout;

namespace Deveel.Webhooks {
	/// <summary>
	/// A default implementation of <see cref="IWebhookSender{TWebhook}"/> that
	/// uses the configured services from the application to send the webhooks
	/// </summary>
	/// <typeparam name="TWebhook">
	/// The type of the webhook to send.
	/// </typeparam>
	/// <remarks>
	/// This service can be constructed with multiple forms for providing a mechanism
	/// to create instances of <see cref="HttpClient"/>, that ultimately will be used
	/// to send the webhooks to the destination: anyway, it is recommended to use the
	/// overloads that accept an instance of <see cref="IHttpClientFactory"/>, to ensure
	/// a proper management of the <see cref="HttpClient"/> instances.
	/// </remarks>
    public class WebhookSender<TWebhook> : WebhookSenderClient, IWebhookSender<TWebhook>, IDisposable where TWebhook : class {
		private readonly IWebhookSignerProvider<TWebhook>? signerProvider;
		private readonly IWebhookJsonSerializer<TWebhook>? jsonSerializer;
		private readonly IWebhookXmlSerializer<TWebhook>? xmlSerializer;
		private readonly IWebhookDestinationVerifier<TWebhook>? verifier;

		/// <summary>
		/// Constructs a new instance of the <see cref="WebhookSender{TWebhook}"/>
		/// </summary>
		/// <param name="options">
		/// A snapshot used to obtain the the options for the sender.
		/// </param>
		/// <param name="httpClientFactory">
		/// A factory used to create instances of <see cref="HttpClient"/>. When this
		/// is <c>null</c> the sender will create a new instance of <see cref="HttpClient"/>
		/// and dispose it when this services is disposed.
		/// </param>
		/// <param name="verifier">
		/// A service that is used to verify the destination of the webhook before
		/// attempting any delivery. This service does not follow the same configuration
		/// and behhavior of the sender, being independently defined, and it is invoked 
		/// before any attempt to send, if the receiver has opted-in for verification.
		/// </param>
		/// <param name="jsonSerializer">
		/// An optional service that is used to serialize the webhook to a JSON string.
		/// When not provided, the sender will use the default JSON serializer.
		/// </param>
		/// <param name="xmlSerializer">
		/// An optional service that is used to serialize the webhook to a XML string.
		/// When not provided, the sender will use the default XML serializer.
		/// </param>
		/// <param name="signerProvider">
		/// An optional service that is used to compute a signature for the webhook. When
		/// the sender options specify that the webhook should be signed, and this service
		/// is not provided, the sender will attempt to use the default signature provider
		/// for a configured algorithm.
		/// </param>
		public WebhookSender(IOptionsSnapshot<WebhookSenderOptions> options,
			IHttpClientFactory httpClientFactory,
			IWebhookDestinationVerifier<TWebhook>? verifier = null,
			IWebhookJsonSerializer<TWebhook>? jsonSerializer = null,
			IWebhookXmlSerializer<TWebhook>? xmlSerializer = null,
			IWebhookSignerProvider<TWebhook>? signerProvider = null)
			: this(options.Get(typeof(TWebhook).Name), httpClientFactory) {
            this.verifier = verifier;
            this.jsonSerializer = jsonSerializer ?? new SystemTextWebhookJsonSerializer<TWebhook>();
			this.xmlSerializer = xmlSerializer ?? new SystemWebhookXmlSerializer<TWebhook>();
			this.signerProvider = signerProvider;
		}

		/// <summary>
		/// Constucts a sender that uses the given options and a HTTP client factory
		/// </summary>
		/// <param name="options">
		/// The instance of the options used to configure the sender.
		/// </param>
		/// <param name="httpClientFactory">
		/// An <see cref="IHttpClientFactory"/> used to create instances of
		/// <see cref="HttpClient"/> used to send webhooks.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the <paramref name="options"/> or the <paramref name="httpClientFactory"/>
		/// are <c>null</c>
		/// </exception>
		protected WebhookSender(WebhookSenderOptions options, IHttpClientFactory httpClientFactory) 
			: base(httpClientFactory) {
			if (options is null) throw new ArgumentNullException(nameof(options));

			SenderOptions = options;
		}

		/// <summary>
		/// Constructs a sender that uses the given options and a HTTP client
		/// </summary>
		/// <param name="options">
		/// The instance of the options used to configure the sender.
		/// </param>
		/// <param name="httpClient">
		/// A <see cref="HttpClient"/> used to send webhooks.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thown when the <paramref name="options"/> or the <paramref name="httpClient"/>
		/// are <c>null</c>.
		/// </exception>
		/// <remarks>
		/// The provided <paramref name="httpClient"/> will not be disposed when
		/// the sender is disposed.
		/// </remarks>
		protected WebhookSender(WebhookSenderOptions options, HttpClient httpClient) : base(httpClient) {
			if (options is null)
				throw new ArgumentNullException(nameof(options));

			if (httpClient is null)
				throw new ArgumentNullException(nameof(httpClient));

			SenderOptions = options;
		}

		/// <summary>
		/// Gets the options used to configure the sender
		/// </summary>
		protected WebhookSenderOptions SenderOptions { get; }

		/// <inheritdoc/>
		protected override WebhookRetryOptions? Retry => SenderOptions.Retry;

		/// <inheritdoc/>
		protected override string? HttpClientName => SenderOptions.HttpClientName;

		/// <inheritdoc/>
		protected override TimeSpan? Timeout => SenderOptions.Timeout;

		/// <summary>
		/// Gets a service that is used to compute the signature of a webhook,
		/// using the algorithm specified.
		/// </summary>
		/// <param name="algorithm">
		/// The signing algorithm of the signer to obtain.
		/// </param>
		/// <returns>
		/// Returns an instance of <see cref="IWebhookSigner"/> that creates
		/// signatures for the specified algorithm, or <c>null</c> if no
		/// instance could be found.
		/// </returns>
		/// <exception cref="ObjectDisposedException">
		/// Thrown when the sender has been disposed
		/// </exception>
        protected virtual IWebhookSigner? GetSigner(string algorithm) {
			ThrowIfDisposed();

			return signerProvider?.GetSigner(algorithm);
		}

		/// <summary>
		/// Creates a signature for the given webhook body, using the specified
		/// algorithm and secret.
		/// </summary>
		/// <param name="algorithm">
		/// The signing algorithm to use.
		/// </param>
		/// <param name="webhookBody">
		/// The string that represents the body of the webhook.
		/// </param>
		/// <param name="secret">
		/// A secret used to sign the webhook.
		/// </param>
		/// <returns>
		/// Returns a string that represents the signature of the webhook.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the <paramref name="algorithm"/>, <paramref name="webhookBody"/> or
		/// <paramref name="secret"/> are <c>null</c> or empty.
		/// </exception>
		protected virtual string? ComputeSignature(string algorithm, string webhookBody, string secret) {
            if (string.IsNullOrWhiteSpace(algorithm))
                throw new ArgumentNullException(nameof(algorithm));
            if (string.IsNullOrWhiteSpace(secret))
                throw new ArgumentNullException(nameof(secret));
            if (string.IsNullOrWhiteSpace(webhookBody))
                throw new ArgumentNullException(nameof(webhookBody));
            
			var signer = GetSigner(algorithm);
			if (signer == null)
				return WebhookSignature.Create(algorithm, webhookBody, secret);

            return signer.SignWebhook(webhookBody, secret);
        }

		/// <summary>
		/// Serializes the given webhook to a JSON string.
		/// </summary>
		/// <param name="webhook">
		/// The instance of <typeparamref name="TWebhook"/> to serialize.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a string that represents the JSON-formatted webhook.
		/// </returns>
		/// <exception cref="NotSupportedException">
		/// If the JSON serialization is not supported by the sender.
		/// </exception>
		/// <exception cref="WebhookSerializationException">
		/// Thrown if the serialization failed through an unhandled error.
		/// </exception>
		protected virtual async Task<string> SerializeToJsonAsync(TWebhook webhook, CancellationToken cancellationToken) {
			if (jsonSerializer == null)
				throw new NotSupportedException("No JSON serializer was set");

			return await jsonSerializer.SerializeToStringAsync(webhook, cancellationToken);
		}

		/// <summary>
		/// Serializes the given webhook to a XML string.
		/// </summary>
		/// <param name="webhook">
		/// The instance of <typeparamref name="TWebhook"/> to serialize.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a string that represents the XML-formatted webhook.
		/// </returns>
		/// <exception cref="NotSupportedException">
		/// If the XML serialization is not supported by the sender.
		/// </exception>
		/// <exception cref="WebhookSerializationException">
		/// Thrown if the serialization failed through an unhandled error.
		/// </exception>
		protected virtual async Task<string> SerializeToXmlAsync(TWebhook webhook, CancellationToken cancellationToken) {
			if (xmlSerializer == null)
				throw new NotSupportedException("No XML serializer was set");

			return await xmlSerializer.SerializeToStringAsync(webhook, cancellationToken);
		}

		/// <summary>
		/// Computes the signature for the given webhook body, using the specified
		/// JSON body and secret, and adds it to the request.
		/// </summary>
		/// <param name="request">
		/// The HTTP request to sign.
		/// </param>
		/// <param name="webhookBody">
		/// The string that represents the body of the webhook.
		/// </param>
		/// <param name="algorithm">
		/// The signing algorithm to use.
		/// </param>
		/// <param name="secret">
		/// A secret used to sign the webhook.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the <paramref name="request"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown when the <paramref name="webhookBody"/>, <paramref name="algorithm"/> or <paramref name="secret"/> 
		/// are <c>null</c> or empty.
		/// </exception>
		/// <exception cref="WebhookSenderException">
		/// Thrown when the signature location is set to <see cref="WebhookSignatureLocation.Header"/>, but
		/// no header name is configured, or when the signature location is set to <see cref="WebhookSignatureLocation.QueryString"/>
		///  and no query parameter name is configured.
		/// </exception>
		protected virtual void SignWebhookRequest(HttpRequestMessage request, string algorithm, string webhookBody, string secret) {
			if (request == null)
				throw new ArgumentNullException(nameof(request));

            if (String.IsNullOrWhiteSpace(algorithm))
                throw new ArgumentException($"'{nameof(algorithm)}' cannot be null or whitespace.", nameof(algorithm));

            if (String.IsNullOrWhiteSpace(webhookBody))
                throw new ArgumentException($"'{nameof(webhookBody)}' cannot be null or whitespace.", nameof(webhookBody));

            var signature = ComputeSignature(algorithm, webhookBody, secret);

			if (SenderOptions.Signature.Location == WebhookSignatureLocation.Header) {
				if (String.IsNullOrWhiteSpace(SenderOptions.Signature.HeaderName))
					throw new WebhookSenderException("The header name for the signature is not set");

				// request.Headers.Add(configuration.DeliveryOptions.SignatureHeaderName, $"{provider.Algorithm}={signature}");
				request.Headers.Add(SenderOptions.Signature.HeaderName, signature);
			} else if (SenderOptions.Signature.Location == WebhookSignatureLocation.QueryString) {
				if (String.IsNullOrWhiteSpace(SenderOptions.Signature.QueryParameter))
                    throw new WebhookSenderException("The query parameter for the signature is not set");

				var uri = request.RequestUri!
					.AddQueryParameter(SenderOptions.Signature.QueryParameter, signature);

				if (!String.IsNullOrWhiteSpace(SenderOptions.Signature.AlgorithmQueryParameter)) {
					uri = uri.AddQueryParameter(SenderOptions.Signature.AlgorithmQueryParameter, algorithm);
				}

				request.RequestUri = uri;
			}
		}

		/// <summary>
		/// Appends a given set of headers to the request.
		/// </summary>
		/// <param name="request">
		/// The HTTP request to add the headers to.
		/// </param>
		/// <param name="headers">
		/// The headers to add to the request.
		/// </param>
		/// <exception cref="WebhookSenderException">
		/// Thrown when the header cannot be added to the request.
		/// </exception>
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

		/// <summary>
		/// Creates a new HTTP request for the given webhook destination.
		/// </summary>
		/// <param name="destination">
		/// The webhook destination to create the request for.
		/// </param>
		/// <returns>
		/// Returns a new HTTP request for the given webhook destination.
		/// </returns>
		protected HttpRequestMessage CreateRequest(WebhookDestination destination) {
			var request = new HttpRequestMessage(HttpMethod.Post, destination.Url);

			AddHeaders(request, destination.Headers);

			return request;
		}

		private async Task<HttpRequestMessage> CreateRequestAsync(WebhookDestination destination, TWebhook webhook, CancellationToken cancellationToken) {
			try {
				var request = CreateRequest(destination);

				string? body = null;
				string? mediaType = null;

				switch (destination.Format ?? SenderOptions.DefaultFormat) {
					case WebhookFormat.Json:
						mediaType = "application/json";
						body = await SerializeToJsonAsync(webhook, cancellationToken);
						break;
					case WebhookFormat.Xml:
						mediaType = "application/xml";
						body = await SerializeToXmlAsync(webhook, cancellationToken);
						break;
					default:
						throw new WebhookSerializationException("Unsupported webhook format");
				}

				if (destination.Sign ?? false && !String.IsNullOrWhiteSpace(destination.Signature.Secret)) {
					if (String.IsNullOrWhiteSpace(SenderOptions.Signature.Algorithm))
						throw new WebhookSenderException("The signature algorithm is not set");

					SignWebhookRequest(request, SenderOptions.Signature.Algorithm, body, destination.Signature!.Secret!);
				}

				request.Content = new StringContent(body, Encoding.UTF8, mediaType);

				return request;
			} catch (WebhookSenderException) {

				throw;
			} catch(Exception ex) {
				throw new WebhookSenderException("An error occurred while creating the request", ex);
			}
		}

		/// <summary>
		/// A callback method that is called when a webhook delivery attempt is started.
		/// </summary>
		/// <param name="destination">
		/// The webhook destination to which the webhhok delivered is attempted.
		/// </param>
		/// <param name="webhook">
		/// The instance of the webhook that is being delivered.
		/// </param>
		/// <param name="attempt">
		/// A description of the webhook delivery attempt.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a task that represents the asynchronous operation.
		/// </returns>
		protected virtual Task OnAttemptStartedAsync(WebhookDestination destination, TWebhook webhook, WebhookDeliveryAttempt attempt, CancellationToken cancellationToken) {
			return Task.CompletedTask;
		}

		/// <summary>
		/// A callback method that is called when a webhook delivery attempt is completed.
		/// </summary>
		/// <param name="destination">
		/// The webhook destination to which the webhhok delivered is attempted.
		/// </param>
		/// <param name="webhook">
		/// The instance of the webhook that is being delivered.
		/// </param>
		/// <param name="attempt">
		/// A description of the webhook delivery attempt and status.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a task that represents the asynchronous operation.
		/// </returns>
		protected virtual Task OnAttemptCompletedAsync(WebhookDestination destination, TWebhook webhook, WebhookDeliveryAttempt attempt, CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }

		private async Task TrySendAsync(WebhookDestination destination, TWebhook webhook, WebhookDeliveryResult<TWebhook> result, CancellationToken cancellationToken) {
			var attempt = result.StartAttempt();
            var timeoutPolicy = CreateTryTimeoutPolicy<HttpResponseMessage>(destination.Retry?.Timeout);

            HttpResponseMessage? response = null;

            await OnAttemptStartedAsync(destination, webhook, attempt, cancellationToken);

            try {
                var request = await CreateRequestAsync(destination, webhook, cancellationToken);

                response = await timeoutPolicy.ExecuteAsync(token => SendRequestAsync(request, token), cancellationToken);

				attempt.Complete((int) response.StatusCode, response.ReasonPhrase);

				response.EnsureSuccessStatusCode();
			} catch (TaskCanceledException) {
                attempt.TimeOut();
                throw;
			} catch (TimeoutException) {
				attempt.TimeOut();
				throw;
			} catch (TimeoutRejectedException ex) {
				attempt.TimeOut();

				throw new TimeoutException("A timeout occurred while trying to get the response", ex);
			} catch (HttpRequestException ex) {
				if (response != null) {
					attempt.Complete((int)response.StatusCode, response.ReasonPhrase);
				} else {
					attempt.LocalFail($"Remote error: {ex.Message}");
				}

				throw;
			} catch (WebhookSenderException ex) {
				attempt.LocalFail($"Local error: {ex.Message}");
				throw;
			} catch (Exception ex) {
				attempt.LocalFail($"Local error: {ex.Message}");
				throw new WebhookSenderException("Could not send the webhook", ex);
			} finally {
				if (!attempt.HasCompleted) {
					attempt.LocalFail("Could not complete the request");
				}

                await OnAttemptCompletedAsync(destination, webhook, attempt, cancellationToken);

				response?.Dispose();
            }
        }

		/// <summary>
		///  Sends a webhook to the given destination.
		/// </summary>
		/// <param name="receiver">
		/// The destination to which the webhook is sent.
		/// </param>
		/// <param name="webhook">
		/// The instance of the webhook that is being sent.
		/// </param>
		/// <param name="cancellationToken">
		/// An optional cancellation token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns an instance of <see cref="WebhookDeliveryResult{TWebhook}"/> that describes the result of the delivery.
		/// </returns>
		/// <exception cref="WebhookVerificationException">
		/// Thrown when the verification of the destination is enabled and it fails
		/// </exception>
		/// <exception cref="WebhookSerializationException">
		/// Thrown when the webhook cannot be serialized
		/// </exception>
		/// <exception cref="WebhookSenderException">
		/// Thrown if any unexpected error occurs while sending the webhook.
		/// </exception>
		/// <remarks>
		/// The general behavior of this implementation is as follows:
		/// <list type="number">
		/// <item>
		/// The sender attempts to send the webhook to the destination, retrying for so many times as specified in the destination:
		/// a first attempt is always made, and then the number of retries specified in the destination specification
		/// (<see cref="WebhookDestination.Retry"/>) or from the sender configuration (see <see cref="WebhookSenderOptions.Retry"/>).
		/// </item>
		/// <item>
		/// A general timeout is applied to the whole operation, if configured in the sender configuration (see <see cref="WebhookSenderOptions.Timeout"/>),
		/// otherwise no timeout is applied.
		/// </item>
		/// </list>
		/// </remarks>
		public virtual async Task<WebhookDeliveryResult<TWebhook>> SendAsync(WebhookDestination receiver, TWebhook webhook, CancellationToken cancellationToken) {
			try {
				var destination = receiver.Merge(SenderOptions);

				var result = new WebhookDeliveryResult<TWebhook>(destination, webhook);

				var timeoutPolicy = CreateTimeoutPolicy();

				var retryPolicy = CreateRetryPolicy(destination.Retry?.MaxRetries, destination.Retry?.MaxDelay);
				var policy = timeoutPolicy.WrapAsync(retryPolicy);

				var captured = await policy.ExecuteAndCaptureAsync(token => TrySendAsync(receiver, webhook, result, token), cancellationToken);

				// TODO: Should we handle the managed state? All the states are in the result object

				if (captured.Outcome == OutcomeType.Failure &&
					captured.FinalException is WebhookSenderException ex) {
					throw ex;
                }

				return result;
			} catch (WebhookSenderException) {

				throw;
			} catch(Exception ex) {
				throw new WebhookSenderException("An error occurred while sending the webhook", ex);
			}
		}
	}
}
