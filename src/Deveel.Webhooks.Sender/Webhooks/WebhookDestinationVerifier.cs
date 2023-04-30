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

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Text;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Polly;
using Polly.Timeout;

namespace Deveel.Webhooks {
	/// <summary>
	/// A default implementation of the <see cref="IWebhookDestinationVerifier{TWebhook}"/>,
	/// that pings a destination URL with some configured parameters to verify if it is reachable.
	/// </summary>
	public class WebhookDestinationVerifier<TWebhook> : WebhookSenderClient, IWebhookDestinationVerifier<TWebhook>, IDisposable
		where TWebhook : class {
		/// <summary>
		/// Creates a new instance of the <see cref="WebhookDestinationVerifier{TWebhook}"/> class
		/// with the given options.
		/// </summary>
		/// <param name="options">
		/// The options to configure the verifier service.
		/// </param>
		/// <param name="httpClientFactory">
		/// An optional factory to create the <see cref="HttpClient"/> instances
		/// used to sende the verification requests.
		/// </param>
		/// <param name="logger">
		/// A logger to use for logging the operations of the verifier.
		/// </param>
		public WebhookDestinationVerifier(IOptions<WebhookSenderOptions<TWebhook>> options, 
			IHttpClientFactory? httpClientFactory = null,
			ILogger<WebhookDestinationVerifier<TWebhook>>? logger = null)
            : this(options.Value, httpClientFactory, logger) {
        }

		/// <summary>
		/// Creates a new instance of the <see cref="WebhookDestinationVerifier{TWebhook}"/> class
		/// that is configured with the given options and an optional factory of HTTP clients.
		/// </summary>
		/// <param name="options">
		/// The options to configure the verifier service.
		/// </param>
		/// <param name="httpClientFactory">
		/// The factory of HTTP clients used to send the verification requests.
		/// </param>
		/// <param name="logger">
		/// A logger to use for logging the operations of the verifier.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the <paramref name="options"/> is <c>null</c>.
		/// </exception>
		protected WebhookDestinationVerifier(WebhookSenderOptions<TWebhook> options, 
			IHttpClientFactory? httpClientFactory = null,
			ILogger? logger = null) 
			: this(options.Verification, httpClientFactory, logger) {
        }

		/// <summary>
		/// Creates a new instance of the <see cref="WebhookDestinationVerifier{TWebhook}"/> class
		/// that is configured with the given options and an optional factory of HTTP clients.
		/// </summary>
		/// <param name="options"></param>
		/// <param name="httpClientFactory"></param>
		/// <param name="logger"></param>
		/// <exception cref="ArgumentNullException"></exception>
		protected WebhookDestinationVerifier(WebhookReceiverVerificationOptions options, 
			IHttpClientFactory? httpClientFactory = null,
			ILogger? logger = null)
			: base(httpClientFactory, logger) {
			VerifierOptions = options ?? throw new ArgumentNullException(nameof(options));
		}

		/// <summary>
		/// Creates a new instance of the <see cref="WebhookDestinationVerifier{TWebhook}"/> class
		/// using an optional HTTP client.
		/// </summary>
		/// <param name="options">
		/// The options to configure the verifier service.
		/// </param>
		/// <param name="httpClient">
		/// The optional HTTP client used to send the verification requests.
		/// </param>
		/// <param name="logger">
		/// A logger to use for logging the operations of the verifier.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the <paramref name="options"/> is <c>null</c>.
		/// </exception>
		protected WebhookDestinationVerifier(WebhookSenderOptions<TWebhook> options, HttpClient httpClient, ILogger? logger) 
			: this(options.Verification, httpClient, logger) {
		}

		/// <summary>
		/// Creates a new instance of the <see cref="WebhookDestinationVerifier{TWebhook}"/> class
		/// that is configured with the given options and an optional HTTP client.
		/// </summary>
		/// <param name="options"></param>
		/// <param name="httpClient"></param>
		/// <param name="logger"></param>
		/// <exception cref="ArgumentNullException"></exception>
		protected WebhookDestinationVerifier(WebhookReceiverVerificationOptions options, HttpClient httpClient, ILogger? logger) 
			: base(httpClient, logger) {
			VerifierOptions = options ?? throw new ArgumentNullException(nameof(options));
		}

		/// <summary>
		/// Gets the options used to configure the verifier service.
		/// </summary>
        protected WebhookReceiverVerificationOptions VerifierOptions { get; }

		/// <inheritdoc/>
		protected override WebhookRetryOptions? Retry => VerifierOptions.Retry;

		/// <inheritdoc/>
		protected override TimeSpan? Timeout => VerifierOptions.Timeout;

		/// <summary>
		/// Appends a challenge to query string of the 
		/// given <paramref name="request"/>
		/// </summary>
		/// <param name="request">
		/// The HTTP request to which the challenge is added.
		/// </param>
		/// <param name="challenge">
		/// The value of the challenge to add to the query string.
		/// </param>
		/// <exception cref="NotSupportedException"></exception>
		protected virtual void AddChallenge(HttpRequestMessage request, string challenge) {
			if (String.IsNullOrWhiteSpace(VerifierOptions.ChallengeQueryParameter))
				throw new NotSupportedException("The challenge query parameter was not set");

			request.RequestUri = request.RequestUri!.AddQueryParameter(VerifierOptions.ChallengeQueryParameter, challenge);
		}

		/// <summary>
		/// Creates a new challenge to be sent to the destination URL.
		/// </summary>
		/// <returns>
		/// Returns a string that represents the challenge to be sent to the 
		/// destination URL.
		/// </returns>
		protected virtual string CreateChallenge() {
			var sb = new StringBuilder();
			for (int i = 0; i < VerifierOptions.ChallengeLength; i++) {
				sb.Append(Random.Shared.Next(0, 9));
			}

			return sb.ToString();
		}

		/// <summary>
		/// Creates a new HTTP request to the given <paramref name="verificationUrl"/>
		/// </summary>
		/// <param name="verificationUrl">
		/// The destination URL to send the verification request.
		/// </param>
		/// <param name="challenge">
		/// A challenge to be sent to the destination URL.
		/// </param>
		/// <param name="token">
		/// The verification token to send in the request.
		/// </param>
		/// <returns>
		/// Returns an instance of <see cref="HttpRequestMessage"/> that can be sent
		/// </returns>
        protected virtual HttpRequestMessage CreateRequest(Uri verificationUrl, string token, string? challenge) {
            var request = new HttpRequestMessage(new HttpMethod(VerifierOptions.HttpMethod), verificationUrl);

            if (VerifierOptions.TokenLocation == VerificationTokenLocation.QueryString) {
				request.RequestUri = request.RequestUri!.AddQueryParameter(VerifierOptions.TokenQueryParameter, token);
            } else {
                request.Headers.TryAddWithoutValidation(VerifierOptions.TokenHeaderName, token);
            }

			if ((VerifierOptions.Challenge ?? false) && 
				!String.IsNullOrWhiteSpace(challenge)) {
				AddChallenge(request, challenge);
			}

            return request;
        }

		private async Task<HttpResponseMessage> VerifyChallengeAsync(HttpResponseMessage response, string challenge, CancellationToken cancellationToken) {
			if (response.Content == null || 
				response.Content.Headers == null ||
				response.Content.Headers.ContentType == null)
				return new HttpResponseMessage(HttpStatusCode.BadRequest);

			if (response.Content.Headers.ContentType.MediaType != "text/plain")
				return new HttpResponseMessage(HttpStatusCode.BadRequest);

			var content = await response.Content.ReadAsStringAsync(cancellationToken);

			if (!String.Equals(content, challenge, StringComparison.Ordinal))
				return new HttpResponseMessage(HttpStatusCode.Unauthorized);

			return new HttpResponseMessage(HttpStatusCode.OK);
		}

		private async Task<HttpStatusCode> TryVerifyAsync(Uri destinationUrl, string verifyToken, string? challenge, CancellationToken cancellationToken) {
			var timeoutPolicy = CreateTryTimeoutPolicy<HttpResponseMessage>(VerifierOptions.Retry?.Timeout);

			HttpResponseMessage? response = null;

			try {
				var request = CreateRequest(destinationUrl, verifyToken, challenge);

				response = await timeoutPolicy.ExecuteAsync(token => SendRequestAsync(request, token), cancellationToken);

				// TODO: Check the response body for a specific value?

				if (response.StatusCode < HttpStatusCode.BadRequest &&
					(VerifierOptions.Challenge ?? false) &&
					!String.IsNullOrWhiteSpace(challenge))
					response = await VerifyChallengeAsync(response, challenge, cancellationToken);

				return response.StatusCode;
			} catch (TimeoutRejectedException ex) {
				throw new TimeoutException("A timeout has occurred", ex);
			} catch (HttpRequestException) {
				throw;
			} catch(TaskCanceledException) {
				throw;
			} catch(Exception ex) {
				throw new WebhookVerificationException("An error occurred while verifying the destination", ex);
			} finally {
				response?.Dispose();
			}
		}

		/// <summary>
		/// Tries to get the verification token from the given parameters.
		/// </summary>
		/// <param name="parameters">
		/// The verification parameters for the destination, from where the 
		/// token should be extracted.
		/// </param>
		/// <param name="verifyToken">
		/// The verification token extracted from the parameters.
		/// </param>
		/// <returns>
		/// Returns <c>true</c> if the token was extracted successfully, otherwise
		/// it returns <c>false</c>.
		/// </returns>
		protected virtual bool TryGetVerifyToken(IDictionary<string, object> parameters, [MaybeNullWhen(false)] out string? verifyToken) {
			if (parameters == null ||
				!parameters.TryGetValue("token", out var token) ||
				token == null) {
				verifyToken = null;
				return false;
			}

			if (!(token is string s))
				s = Convert.ToString(token, CultureInfo.InvariantCulture)!;

			verifyToken = s;
			return true;
		}

		/// <summary>
		/// Verifies the given destination to ensure that it is valid.
		/// </summary>
		/// <param name="destination">
		/// The object that represents the destination to verify.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the verification.
		/// </param>
		/// <returns>
		/// Returns a task that completes with <c>true</c> if the destination is valid,
		/// or <c>false</c> if it is not.
		/// </returns>
		/// <exception cref="WebhookVerificationException">
		/// Thrown when an error occurs while verifying the destination.
		/// </exception>
		public virtual async Task<DestinationVerificationResult> VerifyDestinationAsync(WebhookDestination destination, CancellationToken cancellationToken) {
			try {
				if (destination.Verification == null ||
					destination.Verification.Parameters == null)
					throw new WebhookVerificationException("The destination is not configured to be verified");

				var timeoutPolicy = CreateTimeoutPolicy();

				var retryPolicy = CreateRetryPolicy(destination.Retry?.MaxRetries, destination.Retry?.MaxDelay);
				var policy = timeoutPolicy.WrapAsync(retryPolicy);

				var url = destination.Verification.VerificationUrl ?? destination.Url;

				if (!TryGetVerifyToken(destination.Verification.Parameters, out var verifyToken))
					throw new WebhookVerificationException("It was not possible to find the verification token");

				string? challenge = null;
				if (VerifierOptions.Challenge ?? false)
					challenge = CreateChallenge();

				var capture = await policy.ExecuteAndCaptureAsync(token => TryVerifyAsync(url, verifyToken!, challenge, token), cancellationToken);

				// TODO: configure the expected status code
				if (capture.Outcome == OutcomeType.Successful) {
					var httpStatusCode = (int)capture.Result;

					if (httpStatusCode < 400)
						return DestinationVerificationResult.Success(httpStatusCode);

					return DestinationVerificationResult.Failed(httpStatusCode);
				} else if (capture.FinalException is HttpRequestException error) {
					return DestinationVerificationResult.Failed(error.StatusCode == null ? 0 : (int)error.StatusCode);
				} else if (capture.FinalException is WebhookVerificationException) {
					throw capture.FinalException;
				} else {
					throw new WebhookVerificationException("An error occurred while verifying the destination", capture.FinalException);
				}
			} catch (WebhookVerificationException) {
				throw;
			} catch (Exception ex) {
				throw new WebhookVerificationException("An error occurred while verifying the destination URL", ex);
			}
        }
    }
}
