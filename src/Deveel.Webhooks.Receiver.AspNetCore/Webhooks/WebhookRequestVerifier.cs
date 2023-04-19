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

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	/// <summary>
	/// A default implementation of a verifier of a webhook request that performs
	/// a simple check for a token in the request matching one configured.
	/// </summary>
	/// <typeparam name="TWebhook">
	/// The type of webhook that is being verified
	/// </typeparam>
	public class WebhookRequestVerifier<TWebhook> : IWebhookRequestVerifier<TWebhook>
		where TWebhook : class {
		/// <summary>
		/// Constructs a <see cref="WebhookRequestVerifier{TWebhook}"/> instance with a
		/// selector that resolves the options for the given type of webhook.
		/// </summary>
		/// <param name="options">
		/// The provider of the options for the verification of the webhook request
		/// </param>
		public WebhookRequestVerifier(IOptionsSnapshot<WebhookVerificationOptions> options)
			: this(options.GetVerificationOptions<TWebhook>()) {
		}

		/// <summary>
		/// Constructs a <see cref="WebhookRequestVerifier{TWebhook}"/> instance with the given options
		/// </summary>
		/// <param name="options"></param>
		/// <exception cref="ArgumentNullException"></exception>
		protected WebhookRequestVerifier(WebhookVerificationOptions options) {
			VerificationOptions = options ?? throw new ArgumentNullException(nameof(options));
		}

		/// <summary>
		/// Gets the options for the verification of the webhook request
		/// </summary>
		protected WebhookVerificationOptions VerificationOptions { get; }

		/// <summary>
		/// Responds to the sender with a successful verification of the request.
		/// </summary>
		/// <param name="result">
		/// The result of the verification of the request
		/// </param>
		/// <param name="httpResponse">
		/// The HTTP response object used to respond to the sender
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation
		/// </param>
		/// <returns>
		/// Returns a <see cref="Task"/> that completes when the response is sent
		/// </returns>
		protected async Task OnSuccessAsync(IWebhookVerificationResult result, HttpResponse httpResponse, CancellationToken cancellationToken) {
			httpResponse.StatusCode = VerificationOptions.SuccessStatusCode ?? 200;

			// TODO: Should we emit anything here?
			await httpResponse.WriteAsync("");
		}

		/// <summary>
		/// Responds to the sender with a failed verification of the request.
		/// </summary>
		/// <param name="result">
		/// The failed result of the verification of the request
		/// </param>
		/// <param name="httpResponse">
		/// The HTTP response object used to respond to the sender
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation
		/// </param>
		/// <returns>
		/// Returns a <see cref="Task"/> that completes when the response is sent
		/// </returns>
		protected async Task OnNotAuthenticatedAsync(IWebhookVerificationResult result, HttpResponse httpResponse, CancellationToken cancellationToken) {
			httpResponse.StatusCode = VerificationOptions.NotAuthenticatedStatusCode ?? 403;

			// TODO: Should we emit anything here?
			await httpResponse.WriteAsync("");
		}

		/// <inheritdoc/>
		public virtual async Task HandleResultAsync(IWebhookVerificationResult result, HttpResponse httpResponse, CancellationToken cancellationToken) {
			if (result.IsVerified) {
				await OnSuccessAsync(result, httpResponse, cancellationToken);
			} else {
				await OnNotAuthenticatedAsync(result, httpResponse, cancellationToken);
			}
		}

		/// <summary>
		/// Tries to get the verification token from the given request.
		/// </summary>
		/// <param name="request">
		/// The HTTP request object that carries the data used for the verification
		/// </param>
		/// <param name="token">
		/// A string that contains the token, if found in the request
		/// </param>
		/// <returns>
		/// Returns <c>true</c> if the token is found in the request, or <c>false</c> otherwise
		/// </returns>
		protected virtual bool TryGetVerificationToken(HttpRequest request, [MaybeNullWhen(false)] out string? token) {
			var verificationTokenQueryName = VerificationOptions.VerificationTokenQueryName;

			if (String.IsNullOrWhiteSpace(verificationTokenQueryName)) {
				token = null;
				return false;
			}

			if (!request.Query.TryGetValue(verificationTokenQueryName, out var value)) {
				token = null;
				return false;
			}

			token = value;
			return true;
		}

		/// <inheritdoc/>
		public virtual Task<IWebhookVerificationResult> VerifyRequestAsync(HttpRequest httpRequest, CancellationToken cancellationToken = default) {
			var verificationToken = VerificationOptions.VerificationToken;

			if (!TryGetVerificationToken(httpRequest, out var token) ||
				String.IsNullOrWhiteSpace(verificationToken) ||
				!String.Equals(token, verificationToken, StringComparison.Ordinal))
				return Task.FromResult<IWebhookVerificationResult>(WebhookVerificationResult.Failed);

			return Task.FromResult<IWebhookVerificationResult>(WebhookVerificationResult.Verified);
		}
	}
}
