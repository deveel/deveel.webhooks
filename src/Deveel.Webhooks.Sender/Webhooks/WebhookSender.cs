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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
	public class WebhookSender<TWebhook> : IWebhookSender<TWebhook> where TWebhook : class {
		private readonly IHttpClientFactory httpClientFactory;
		private readonly ILogger logger;

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
		/// <param name="logger">
		/// A logger used to log messages during the sending of webhooks.
		/// </param>
		public WebhookSender(IOptions<WebhookSenderOptions<TWebhook>> options,
			IHttpClientFactory httpClientFactory,
			ILogger<WebhookSender<TWebhook>>? logger = null)
			: this(options.Value, httpClientFactory, logger) {
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
		/// <param name="logger">
		/// A logger that is used to log messages during the sending of webhooks.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the <paramref name="options"/> or the <paramref name="httpClientFactory"/>
		/// are <c>null</c>
		/// </exception>
		protected WebhookSender(WebhookSenderOptions<TWebhook> options, 
			IHttpClientFactory httpClientFactory,
			ILogger<WebhookSender<TWebhook>>? logger = null) {
			ArgumentNullException.ThrowIfNull(options, nameof(options));

			this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

			SenderOptions = options;
			this.logger = logger ?? NullLogger<WebhookSender<TWebhook>>.Instance;
		}

		/// <summary>
		/// Gets the options used to configure the sender
		/// </summary>
		protected WebhookSenderOptions<TWebhook> SenderOptions { get; }

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
			if (SenderOptions.Signer != null &&
				SenderOptions.Signer.Algorithms.Any(x => String.Equals(x, algorithm, StringComparison.OrdinalIgnoreCase)))
				return SenderOptions.Signer;

			return null;
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

			logger.TraceCreatingSignature(algorithm);

			string? signature = null;

			var signer = GetSigner(algorithm);
			if (signer == null) {
				signature = WebhookSignature.Create(algorithm, webhookBody, secret);
			} else {
				signature = signer.SignWebhook(webhookBody, secret);
			}

			logger.TraceCreatingSignature(algorithm);

			return signature;
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
			if (SenderOptions.JsonSerializer == null)
				throw new NotSupportedException("No JSON serializer was set");

			logger.TraceSerializing("json");

			var result = await SenderOptions.JsonSerializer.SerializeToStringAsync(webhook, cancellationToken);

			logger.TraceSerialized("json");

			return result;
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
			if (SenderOptions.XmlSerializer == null)
				throw new NotSupportedException("No XML serializer was set");

			logger.TraceSerializing("xml");

			var result = await SenderOptions.XmlSerializer.SerializeToStringAsync(webhook, cancellationToken);

			logger.TraceSerialized("xml");

			return result;
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

				logger.TraceSigningRequest(algorithm, WebhookSignatureLocation.Header, SenderOptions.Signature.HeaderName);

				// request.Headers.Add(configuration.DeliveryOptions.SignatureHeaderName, $"{provider.Algorithm}={signature}");
				request.Headers.Add(SenderOptions.Signature.HeaderName, signature);

				logger.TraceRequestSigned(algorithm, WebhookSignatureLocation.Header, SenderOptions.Signature.HeaderName);
			} else if (SenderOptions.Signature.Location == WebhookSignatureLocation.QueryString) {
				if (String.IsNullOrWhiteSpace(SenderOptions.Signature.QueryParameter))
                    throw new WebhookSenderException("The query parameter for the signature is not set");

				logger.TraceSigningRequest(algorithm, WebhookSignatureLocation.QueryString, SenderOptions.Signature.QueryParameter);

				var uri = request.RequestUri!
					.AddQueryParameter(SenderOptions.Signature.QueryParameter, signature);

				if (!String.IsNullOrWhiteSpace(SenderOptions.Signature.AlgorithmQueryParameter)) {
					uri = uri.AddQueryParameter(SenderOptions.Signature.AlgorithmQueryParameter, algorithm);
				}

				request.RequestUri = uri;

				logger.TraceRequestSigned(algorithm, WebhookSignatureLocation.QueryString, SenderOptions.Signature.QueryParameter);
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
		/// Adds the trace header to the request, if enabled.
		/// </summary>
		/// <param name="request">
		/// The HTTP request to add the trace header to.
		/// </param>
		/// <param name="traceId">
		/// The unique identifier of the current operation.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the <paramref name="request"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="WebhookSenderException">
		/// Thrown when the trace header cannot be added to the request.
		/// </exception>
		protected virtual void AddTraceHeader(HttpRequestMessage request, string traceId) {
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			if (SenderOptions.AddTraceHeaders ?? false) {
				var headerName = String.IsNullOrWhiteSpace(SenderOptions.TraceHeaderName) ? 
					WebhookSenderDefaults.TraceHeaderName : 
					SenderOptions.TraceHeaderName;

				if (!request.Headers.TryAddWithoutValidation(headerName, traceId))
					throw new WebhookSenderException("Could not add the trace header to the request");
			}
		}

		/// <summary>
		/// Adds the attempt header to the request, if enabled.
		/// </summary>
		/// <param name="request">
		/// The HTTP request to add the attempt header to.
		/// </param>
		/// <param name="attempt">
		/// The attempt number to add to the header.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the <paramref name="request"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="WebhookSenderException">
		/// Thrown when the attempt header cannot be added to the request.
		/// </exception>
		protected virtual void AddAttemptHeader(HttpRequestMessage request, int attempt) {
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			if (SenderOptions.AddTraceHeaders ?? false) {
				var headerName = String.IsNullOrWhiteSpace(SenderOptions.AttemptTraceHeaderName) ?
					WebhookSenderDefaults.AttemptTraceHeaderName :
					SenderOptions.AttemptTraceHeaderName;

				if (!request.Headers.TryAddWithoutValidation(headerName, attempt.ToString()))
					throw new WebhookSenderException("Could not add the attempt header to the request");
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
						mediaType = WebhookSenderDefaults.JsonContentType;
						body = await SerializeToJsonAsync(webhook, cancellationToken);
						break;
					case WebhookFormat.Xml:
						mediaType = WebhookSenderDefaults.XmlContentType;
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

		private async Task<HttpResponseMessage> TrySendAsync(HttpClient httpClient, WebhookDestination destination, TWebhook webhook, WebhookDeliveryResult<TWebhook> result, CancellationToken cancellationToken) {
			var attempt = result.StartAttempt();

            HttpResponseMessage? response = null;

            await OnAttemptStartedAsync(destination, webhook, attempt, cancellationToken);

			HttpRequestMessage? request = null;

            try {
                request = await CreateRequestAsync(destination, webhook, cancellationToken);

				AddTraceHeader(request, result.OperationId);
				AddAttemptHeader(request, attempt.Number);

				logger.TraceStartingAttempt(request.RequestUri!.GetLeftPart(UriPartial.Path));

				var timeoutPolicy = destination.CreateTimeoutPolicy(SenderOptions.Retry);

                response = await timeoutPolicy.ExecuteAsync(token => httpClient.SendAsync(request, token), cancellationToken);

				attempt.Complete((int) response.StatusCode, response.ReasonPhrase);

				response.EnsureSuccessStatusCode();

				return response;
			} catch (TaskCanceledException ex) {
				logger.WarnTimeOut(ex);
                attempt.TimeOut();
                throw;
			} catch (TimeoutException ex) {
				logger.WarnTimeOut(ex);
				attempt.TimeOut();
				throw;
			} catch (TimeoutRejectedException ex) {
				logger.WarnTimeOut(ex);
				attempt.TimeOut();

				throw new TimeoutException("A timeout occurred while trying to get the response", ex);
			} catch (HttpRequestException ex) {
				logger.WarnRequestFailed(ex, request?.RequestUri!.GetLeftPart(UriPartial.Path)!, (int?)(response?.StatusCode ?? ex.StatusCode));

				if (response != null) {
					attempt.Complete((int)response.StatusCode, response.ReasonPhrase);
				} else {
					attempt.LocalFail($"Remote error: {ex.Message}");
				}

				throw;
			} catch (WebhookSenderException ex) {
				logger.LogUnknownError(ex);

				attempt.LocalFail($"Local error: {ex.Message}");
				throw;
			} catch (Exception ex) {
				logger.LogUnknownError(ex);

				attempt.LocalFail($"Local error: {ex.Message}");
				throw new WebhookSenderException("Could not send the webhook", ex);
			} finally {
				if (!attempt.HasCompleted) {
					attempt.LocalFail("Could not complete the request");
				}

				logger.TraceAttemptFinished(request?.RequestUri!.GetLeftPart(UriPartial.Path)!, (int?)attempt.ResponseCode);

				if (attempt.Failed) {
					// TODO: log that the attempt failed
				} else if (attempt.Succeeded) {
					// TODO: log that the attempt succeeded
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
		/// (<see cref="WebhookDestination.Retry"/>) or from the sender configuration (see <see cref="WebhookSenderOptions{TWebhook}.Retry"/>).
		/// </item>
		/// <item>
		/// A general timeout is applied to the whole operation, if configured in the sender configuration (see <see cref="WebhookSenderOptions{TWebhook}.Timeout"/>),
		/// otherwise no timeout is applied.
		/// </item>
		/// </list>
		/// </remarks>
		public virtual async Task<WebhookDeliveryResult<TWebhook>> SendAsync(WebhookDestination receiver, TWebhook webhook, CancellationToken cancellationToken) {
			try {
				var destination = receiver.Merge(SenderOptions);

				logger.TraceSendingWebhook(destination.Url.GetLeftPart(UriPartial.Path));

				var httpClient = httpClientFactory.CreateClient(destination.Name);

				var operationId = Guid.NewGuid().ToString("N");
				var result = new WebhookDeliveryResult<TWebhook>(operationId, destination, webhook);

				var retryPolicy = destination.CreateRetryPolicy(SenderOptions.Retry);
				var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(SenderOptions.Timeout ?? Timeout.InfiniteTimeSpan);

				var policy = Policy.WrapAsync(timeoutPolicy, retryPolicy);

				var captured = await policy.ExecuteAndCaptureAsync(token => TrySendAsync(httpClient, receiver, webhook, result, token), cancellationToken);

				// TODO: Should we handle the managed state? All the states are in the result object

				if (captured.Outcome == OutcomeType.Failure &&
					captured.FinalException is WebhookSenderException ex) {
					throw ex;
                }

				if (result.Successful) {
					logger.TraceSuccessfulDelivery(destination.Url.GetLeftPart(UriPartial.Path));
				} else {
					logger.WarnDeliveryFailed(destination.Url.GetLeftPart(UriPartial.Path));
				}

				return result;
			} catch (WebhookSenderException) {

				throw;
			} catch(Exception ex) {
				logger.LogUnknownError(ex);

				throw new WebhookSenderException("An error occurred while sending the webhook", ex);
			}
		}
	}
}
