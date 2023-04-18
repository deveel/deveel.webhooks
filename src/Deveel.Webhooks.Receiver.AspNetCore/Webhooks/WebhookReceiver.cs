using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	public partial class WebhookReceiver<TWebhook> : IWebhookReceiver<TWebhook>
		where TWebhook : class {
		private readonly IWebhookSignerProvider<TWebhook> signerProvider;

		public WebhookReceiver(IOptions<WebhookReceiverOptions<TWebhook>> options,
			IWebhookSignerProvider<TWebhook> signerProvider = null,
			IWebhookJsonParser<TWebhook> jsonParser = null)
			: this(options.Value, jsonParser) {
			this.signerProvider = signerProvider;
		}

		protected WebhookReceiver(WebhookReceiverOptions<TWebhook> receiverOptions, IWebhookJsonParser<TWebhook> jsonParser) {
			ReceiverOptions = receiverOptions ?? throw new ArgumentNullException(nameof(receiverOptions));
			JsonParser = jsonParser;
		}

		protected WebhookReceiverOptions<TWebhook> ReceiverOptions { get; }

		protected IWebhookJsonParser<TWebhook> JsonParser { get; }

		protected virtual IWebhookSigner GetSigner(string algorithm) {
			return signerProvider?.GetSigner(algorithm);
		}

		protected virtual string SignWebhook(string jsonBody, string algorithm, string secret) {
			var signer = GetSigner(algorithm);
			if (signer == null)
				return null;

			return signer.SignWebhook(jsonBody, secret);
		}

		protected virtual async Task<TWebhook> ParseJsonAsync(string jsonBody, CancellationToken cancellationToken) {
			using var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonBody));
			return await ParseJsonAsync(stream, cancellationToken);
		}

		protected virtual async Task<TWebhook> ParseJsonAsync(Stream utf8Stream, CancellationToken cancellationToken) {
			if (JsonParser == null)
				throw new NotSupportedException("The JSON parser was not provided");

			return await JsonParser.ParseWebhookAsync(utf8Stream, cancellationToken);
		}

		private int InvalidSignatureStatusCode() => ReceiverOptions.Signature?.InvalidStatusCode ?? 400;

		private bool ValidateSignature()
			=> (ReceiverOptions.VerifySignature ?? false) && 
			ReceiverOptions.Signature != null &&
			!String.IsNullOrWhiteSpace(ReceiverOptions.Signature.ParameterName) &&
			!String.IsNullOrWhiteSpace(ReceiverOptions.Signature.Secret) &&
			!String.IsNullOrWhiteSpace(ReceiverOptions.Signature.Algorithm);

		protected virtual bool TryGetSignature(HttpRequest request, out string signature) {
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

		protected virtual bool IsSignatureValid(string signature, string algorithm, string jsonBody) {
			if (!(ReceiverOptions.VerifySignature ?? false))
				return true;

			var computedSignature = SignWebhook(jsonBody, algorithm, ReceiverOptions.Signature.Secret);
			if (String.IsNullOrWhiteSpace(computedSignature))
				return false;

			return String.Equals(computedSignature, signature, StringComparison.OrdinalIgnoreCase);
		}

		protected async Task<ValidateResult> TryValidateWebhook(HttpRequest request) {
			using var reader = new StreamReader(request.Body, Encoding.UTF8);
			var jsonBody = await reader.ReadToEndAsync();

			if (!ValidateSignature() ||
				!TryGetSignature(request, out var signature))
				return new ValidateResult(jsonBody, false, null);

			var isValid = IsSignatureValid(signature, ReceiverOptions.Signature.Algorithm, jsonBody);

			return new ValidateResult(jsonBody, true, isValid);
		}

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
			} catch (WebhookException) {
				throw;
			} catch(Exception ex) {
				throw new WebhookException("Could not receive the webhook", ex);
			}
		}

		protected readonly struct ValidateResult {
			public bool SignatureValidated { get; }

			public bool? IsValid { get; }

			public string JsonBody { get; }

			public ValidateResult(string jsonBody, bool validated, bool? isValid) : this() {
				JsonBody = jsonBody;
				SignatureValidated = validated;
				IsValid = isValid;
			}
		}

	}
}
