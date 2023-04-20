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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;

using Microsoft.Extensions.Options;

using Polly;
using Polly.Timeout;

namespace Deveel.Webhooks {
    /// <summary>
    /// A default implementation of the <see cref="IWebhookDestinationVerifier"/>,
    /// that pings a destination URL with some configured parameters to verify if it is reachable.
    /// </summary>
    public class WebhookDestinationVerifier : WebhookSenderClient, IWebhookDestinationVerifier, IDisposable {
		/// <summary>
		/// Creates a new instance of the <see cref="WebhookDestinationVerifier"/> class
		/// with the given options.
		/// </summary>
		/// <param name="options">
		/// The options to configure the verifier service.
		/// </param>
		/// <param name="httpClientFactory">
		/// An optional factory to create the <see cref="HttpClient"/> instances
		/// used to sende the verification requests.
		/// </param>
        public WebhookDestinationVerifier(IOptions<WebhookDestinationVerifierOptions> options, 
			IHttpClientFactory? httpClientFactory = null)
            : this(options.Value, httpClientFactory) {
        }

		/// <summary>
		/// Creates a new instance of the <see cref="WebhookDestinationVerifier"/> class
		/// that is configured with the given options and an optional factory of HTTP clients.
		/// </summary>
		/// <param name="options">
		/// The options to configure the verifier service.
		/// </param>
		/// <param name="httpClientFactory">
		/// The factory of HTTP clients used to send the verification requests.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the <paramref name="options"/> is <c>null</c>.
		/// </exception>
        protected WebhookDestinationVerifier(WebhookDestinationVerifierOptions options, IHttpClientFactory? httpClientFactory = null) 
			: base(httpClientFactory) {
            VerifierOptions = options ?? throw new ArgumentNullException(nameof(options));
        }

		/// <summary>
		/// Creates a new instance of the <see cref="WebhookDestinationVerifier"/> class
		/// using an optional HTTP client.
		/// </summary>
		/// <param name="options">
		/// The options to configure the verifier service.
		/// </param>
		/// <param name="httpClient">
		/// The optional HTTP client used to send the verification requests.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the <paramref name="options"/> is <c>null</c>.
		/// </exception>
		protected WebhookDestinationVerifier(WebhookDestinationVerifierOptions options, HttpClient httpClient) : base(httpClient) {
			VerifierOptions = options ?? throw new ArgumentNullException(nameof(options));
		}

		/// <summary>
		/// Gets the options used to configure the verifier service.
		/// </summary>
        protected WebhookDestinationVerifierOptions VerifierOptions { get; }

		/// <inheritdoc/>
		protected override WebhookRetryOptions? Retry => VerifierOptions.Retry;

		/// <inheritdoc/>
		protected override TimeSpan? Timeout => VerifierOptions.Timeout;

		/// <summary>
		/// Creates a new HTTP request to the given <paramref name="verificationUrl"/>
		/// </summary>
		/// <param name="verificationUrl">
		/// The destination URL to send the verification request.
		/// </param>
		/// <param name="token">
		/// The verification token to send in the request.
		/// </param>
		/// <returns>
		/// Returns an instance of <see cref="HttpRequestMessage"/> that can be sent
		/// </returns>
        protected virtual HttpRequestMessage CreateRequest(Uri verificationUrl, string token) {
            var request = new HttpRequestMessage(new HttpMethod(VerifierOptions.HttpMethod), verificationUrl);

            if (VerifierOptions.TokenLocation == VerificationTokenLocation.QueryString) {
				request.RequestUri = request.RequestUri!.AddQueryParameter(VerifierOptions.TokenQueryParameter, token);
            } else {
                request.Headers.TryAddWithoutValidation(VerifierOptions.TokenHeaderName, token);
            }

            return request;
        }

		private async Task<HttpStatusCode> TryVerifyAsync(Uri destinationUrl, string verifyToken, CancellationToken cancellationToken) {
			var timeoutPolicy = CreateTryTimeoutPolicy<HttpResponseMessage>(VerifierOptions.Retry?.Timeout);

			HttpResponseMessage? response = null;

			try {
				var request = CreateRequest(destinationUrl, verifyToken);

				response = await timeoutPolicy.ExecuteAsync(token => SendRequestAsync(request, token), cancellationToken);

				// TODO: Check the response body for a specific value?

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
		public virtual async Task<bool> VerifyDestinationAsync(WebhookDestination destination, CancellationToken cancellationToken) {
			try {
				if (!(destination.Verify ?? false) ||
					destination.Verification == null ||
					destination.Verification.Parameters == null)
					throw new WebhookVerificationException("The destination is not configured to be verified");

				var timeoutPolicy = CreateTimeoutPolicy();

				var retryPolicy = CreateRetryPolicy(destination.Retry?.MaxRetries, destination.Retry?.MaxDelay);
				var policy = timeoutPolicy.WrapAsync(retryPolicy);

				var url = destination.Verification.VerificationUrl ?? destination.Url;

				if (!TryGetVerifyToken(destination.Verification.Parameters, out var verifyToken))
					throw new WebhookVerificationException("It was not possible to find the verification token");

				var capture = await policy.ExecuteAndCaptureAsync(token => TryVerifyAsync(url, verifyToken!, token), cancellationToken);

				// TODO: configure the expected status code
				if (capture.Outcome == OutcomeType.Successful) {
					return (int)capture.Result < 400;
				} else if (capture.FinalException is HttpRequestException error) {
					return false;
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
