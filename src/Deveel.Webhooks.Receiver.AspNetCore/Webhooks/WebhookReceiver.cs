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
using System.Text;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
    /// <summary>
    /// A default implementation of the <see cref="IWebhookReceiver{TWebhook}"/>
    /// that uses the registered options and services to receive a webhook.
    /// </summary>
    /// <typeparam name="TWebhook">The type of the webhook to receive</typeparam>
    /// <remarks>
    /// <para>
    /// This class implements a default behavior for the <see cref="IWebhookReceiver{TWebhook}"/>,
    /// that is based on common patterns for the processing of webhooks.
    /// </para>
	/// <para>
	/// It is recommended to inherit from this class to implement a custom receiver behavior,
	/// when the default behavior is not sufficient. In some case scenarios, it is recommended
	/// to discard the possibility of using this class and implement the <see cref="IWebhookReceiver{TWebhook}"/>.
	/// </para>
    /// </remarks>
    public class WebhookReceiver<TWebhook> : IWebhookReceiver<TWebhook>
		where TWebhook : class {
		private readonly IWebhookSignerProvider<TWebhook>? signerProvider;

		/// <summary>
		/// Constructs a <see cref="WebhookReceiver{TWebhook}"/> instance.
		/// </summary>
		/// <param name="options">An instance of the <see cref="IOptionsSnapshot{TOptions}"/> that is
		/// used to resolve the configurations specific for this receiver.</param>
		/// <param name="signerProvider">A provider of <see cref="IWebhookSigner"/> services that
		/// are used to verify the signature of webhooks</param>
		/// <param name="jsonParser">A parser that is used to process the JSON
		/// content of requests and obtain instances of webhooks. By default, if this
		/// value is <c>null</c> a new instance of <see cref="SystemTextWebhookJsonParser{TWebhook}"/>
		/// is created using the default options.</param>
		public WebhookReceiver(IOptionsSnapshot<WebhookReceiverOptions> options,
            IWebhookJsonParser<TWebhook>? jsonParser = null,
            IWebhookSignerProvider<TWebhook>? signerProvider = null)
			: this(options.GetReceiverOptions<TWebhook>(), jsonParser) {
			this.signerProvider = signerProvider;
		}

        /// <summary>
        /// Constructs a <see cref="WebhookReceiver{TWebhook}"/> instance.
        /// </summary>
        /// <param name="options">The configurations used by the receiver to
        /// process the requests</param>
        /// <param name="jsonParser">A parser that is used to process the JSON
        /// content of requests and obtain instances of webhooks. By default, if this
        /// value is <c>null</c> a new instance of <see cref="SystemTextWebhookJsonParser{TWebhook}"/>
        /// is created using the default options.</param>
        /// <exception cref="ArgumentNullException">
		/// Thrown if the given <paramref name="options"/> is <c>null</c>
		/// </exception>
        protected WebhookReceiver(WebhookReceiverOptions options, IWebhookJsonParser<TWebhook>? jsonParser) {
			ReceiverOptions = options ?? throw new ArgumentNullException(nameof(options));
			JsonParser = jsonParser ?? new SystemTextWebhookJsonParser<TWebhook>();
		}

		/// <summary>
		/// Gets the options used by the receiver to process the requests.
		/// </summary>
		protected virtual WebhookReceiverOptions ReceiverOptions { get; }

		/// <summary>
		/// Gets the parser used to process the JSON content of requests
		/// </summary>
		protected virtual IWebhookJsonParser<TWebhook> JsonParser { get; }

		/// <summary>
		/// Resolves a webhook signer for the given algorithm.
		/// </summary>
		/// <param name="algorithm">The hashing algorithm used to sign the webhook</param>
		/// <returns>
		/// Returns an instance of <see cref="IWebhookSigner"/> that is used to
		/// sign the webhook, or <c>null</c> if no signer is available for the
		/// given algorithm.
		/// </returns>
		protected virtual IWebhookSigner? GetSigner(string algorithm) {
			return signerProvider?.GetSigner(algorithm);
		}

		/// <summary>
		/// Signs the JSON body of a webhook using the given algorithm and secret.
		/// </summary>
		/// <param name="jsonBody">The JSON-formatted representation of a webhook</param>
		/// <param name="algorithm">The hashing algorithm used to sign the webhook</param>
		/// <param name="secret">A secret word used to compute the signature</param>
		/// <returns>
		/// Returns a string that is the signature of the given JSON body, or <c>null</c>
		/// if no signer is available for the given algorithm.
		/// </returns>
		protected virtual string? SignWebhook(string jsonBody, string algorithm, string secret) {
			return GetSigner(algorithm)?.SignWebhook(jsonBody, secret);
		}

		/// <summary>
		/// Parses the JSON body of a webhook request.
		/// </summary>
		/// <param name="jsonBody">The JSON-formatted body of the webhook to be parsed</param>
		/// <param name="cancellationToken"></param>
		/// <returns>
		/// Returns an instance of <typeparamref name="TWebhook"/> that completes the
		/// parsing operation to obtain the webhook.
		/// </returns>
		/// <exception cref="NotSupportedException">
		/// Thrown if the parsing operation is not supported by the receiver.
		/// </exception>
		protected virtual async Task<TWebhook?> ParseJsonAsync(string? jsonBody, CancellationToken cancellationToken) {
            if (JsonParser == null)
                throw new NotSupportedException("The JSON parser was not provided");

			return await JsonParser.ParseWebhookAsync(jsonBody, cancellationToken);
		}

        /// <summary>
        /// Parses the JSON body of a webhook request.
        /// </summary>
        /// <param name="utf8Stream">A stream that is UTF-8 encoded and that provides the
        /// body of the webhook to be parsed</param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// Returns an instance of <typeparamref name="TWebhook"/> that completes the
        /// parsing operation to obtain the webhook.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Thrown if the parsing operation is not supported by the receiver.
        /// </exception>
        protected virtual async Task<TWebhook?> ParseJsonAsync(Stream utf8Stream, CancellationToken cancellationToken) {
			if (JsonParser == null)
				throw new NotSupportedException("The JSON parser was not provided");

			return await JsonParser.ParseWebhookAsync(utf8Stream, cancellationToken);
		}

		private string? GetAlgorithm(string signature) {
            var index = signature.IndexOf('=');
			if (index == -1)
				return ReceiverOptions?.Signature?.Algorithm;

			return signature.Substring(0, index);
        }

		private bool ValidateSignature()
			=> (ReceiverOptions.VerifySignature ?? false) && 
			ReceiverOptions.Signature != null &&
			!String.IsNullOrWhiteSpace(ReceiverOptions.Signature.ParameterName) &&
			!String.IsNullOrWhiteSpace(ReceiverOptions.Signature.Secret);

		/// <summary>
		/// Attempts to get the signature from the given request.
		/// </summary>
		/// <param name="request">The HTTP request from the sender of the webhook
		/// that should include a signature</param>
		/// <param name="signature">The signature of the webhook discovered from within
		/// the request</param>
		/// <remarks>
		/// By default this method verifies if the configuration of the receiver
		/// explicitly requires or forbids the verification of signatures: in the
		/// cases the receiver is configured not to verify signatures, this method
		/// will return <c>false</c> even if the signature is present in the request.
		/// </remarks>
		/// <returns>
		/// Returns <c>true</c> if the signature was found in the request, or <c>false</c> otherwise.
		/// </returns>
		protected virtual bool TryGetSignature(HttpRequest request, [MaybeNullWhen(false)] out string? signature) {
			if (!ValidateSignature()) {
				signature = null;
				return false;
			}

			if (ReceiverOptions.Signature.Location == WebhookSignatureLocation.Header) {
				if (!request.Headers.TryGetValue(ReceiverOptions.Signature.ParameterName, out var header)) {
					signature = null;
					return false;
				}

				signature = header.ToString();
				return true;
			} else if (ReceiverOptions.Signature.Location == WebhookSignatureLocation.QueryString) {
				if (!request.Query.TryGetValue(ReceiverOptions.Signature.ParameterName, out var value)) {
					signature = null;
					return false;
				}

				signature = value.ToString();
				return true;
			}

			signature = null;
			return false;
		}

		/// <summary>
		/// Verifies if the given signature sent alongside a webhook is 
		/// valid for the given JSON body of the webhook itself.
		/// </summary>
		/// <param name="signature">The signature sent alongside the webhook</param>
		/// <param name="algorithm">The signing hash algorithm used to compute the signature</param>
		/// <param name="jsonBody">The JSON-formatted body of the webhook</param>
		/// <remarks>
		/// <para>
		/// The default behavior of this method is to return <c>true</c> if the verification 
		/// of the signature is disabled in the configuration of the receiver.
		/// </para>
		/// <para>
		/// To verify the signature, this method will use the secret word configured as a key
		/// to compute the signature of the given JSON body, and then compare it with the one
		/// sent alongside the webhook.
		/// </para>
		/// </remarks>
		/// <returns>
		/// Returns <c>true</c> if the signature is valid for the given webhook, 
		/// or <c>false</c> otherwise.
		/// </returns>
		protected virtual bool IsSignatureValid(string signature, string algorithm, string jsonBody) {
			if (!ValidateSignature())
				return true;

			if (String.IsNullOrWhiteSpace(ReceiverOptions?.Signature?.Secret))
				return false;

			var computedSignature = SignWebhook(jsonBody, algorithm, ReceiverOptions.Signature.Secret);
			if (String.IsNullOrWhiteSpace(computedSignature))
				return false;

			return String.Equals(computedSignature, signature, StringComparison.Ordinal);
		}

		/// <summary>
		/// Attempts to validate the webhook request.
		/// </summary>
		/// <param name="request">The HTTP request used to post the webhook</param>
		/// <returns>
		/// Returns a <see cref="ValidateResult"/> that describes the result of the validation.
		/// </returns>
		protected async Task<ValidateResult> TryValidateWebhook(HttpRequest request) {
			using var reader = new StreamReader(request.Body, Encoding.UTF8);
			var jsonBody = await reader.ReadToEndAsync();

			if (!ValidateSignature() ||
				!TryGetSignature(request, out var signature) ||
				String.IsNullOrWhiteSpace(signature))
				return new ValidateResult(jsonBody, false, null);

			var algorithm = GetAlgorithm(signature);
			if (String.IsNullOrWhiteSpace(algorithm))
				return new ValidateResult(jsonBody, true, false);

			var isValid = IsSignatureValid(signature, algorithm, jsonBody);

			return new ValidateResult(jsonBody, true, isValid);
		}

		/// <inheritdoc/>
		public virtual async Task<WebhookReceiveResult<TWebhook>> ReceiveAsync(HttpRequest request, CancellationToken cancellationToken) {
			if (String.IsNullOrWhiteSpace(request.ContentType) ||
				!request.ContentType.StartsWith("application/json"))
				return new WebhookReceiveResult<TWebhook>(null, null);

			try {
				if (ValidateSignature()) {
					var result = await TryValidateWebhook(request);

					if (result.SignatureValidated && !(result.IsValid ?? false)) {
						return new WebhookReceiveResult<TWebhook>(null, false);
					} else if ((result.SignatureValidated && (result.IsValid ?? false)) ||
						!result.SignatureValidated) {
						var signatureValid = result.SignatureValidated && (result.IsValid ?? false);
						var webhook = await ParseJsonAsync(result.JsonBody, cancellationToken);
						return new WebhookReceiveResult<TWebhook>(webhook, signatureValid);
					} else {
						throw new NotSupportedException();
					}
				} else {
					return await ParseJsonAsync(request.Body, cancellationToken);
				}
			} catch (WebhookReceiverException) {
				throw;
			} catch(Exception ex) {
				throw new WebhookReceiverException("Could not receive the webhook", ex);
			}
		}

		/// <summary>
		/// Describes the result of a validation attempt.
		/// </summary>
		protected readonly struct ValidateResult {
			/// <summary>
			/// Indicates if the signature was actually validated.
			/// </summary>
			public bool SignatureValidated { get; }

			/// <summary>
			/// Indicates if the signature was valid, or <c>null</c> if the
			/// signature was not validated.
			/// </summary>
			public bool? IsValid { get; }

			/// <summary>
			/// Gets the JSON body of the webhook, or <c>null</c> if it was
			/// not possible to read it from the request.
			/// </summary>
			public string? JsonBody { get; }

			/// <summary>
			/// Initializes a new instance of the <see cref="ValidateResult"/> struct.
			/// </summary>
			/// <param name="jsonBody">The JSON-formatted string that represents the webhook</param>
			/// <param name="validated">Indicates if the webhook signature was actually validated</param>
			/// <param name="isValid">Indicates if the webhook signature was valid</param>
			public ValidateResult(string? jsonBody, bool validated, bool? isValid) : this() {
				JsonBody = jsonBody;
				SignatureValidated = validated;
				IsValid = isValid;
			}
		}

	}
}
