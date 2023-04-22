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

namespace Deveel.Webhooks {
	// TODO: Find a better name for this class? The reason
	//    for not calling it 'WebhookReceiver' is to avoid
	//    naming conflicts with the 'WebhookReceiver' class
	//    in the 'Deveel.Webhooks.Receiver' project.

	/// <summary>
	/// Describes the delivery destination of a webhook.
	/// </summary>
	/// <remarks>
	/// This class is used to describe the destination of a webhook,
	/// including configuration options for the delivery and verification.
	/// </remarks>
    public sealed class WebhookDestination {
		/// <summary>
		/// Initializes a new instance of the <see cref="WebhookDestination"/> class
		/// </summary>
		/// <param name="url">
		/// The absolute URL of the destination.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the <paramref name="url"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown when the <paramref name="url"/> is not an absolute URI.
		/// </exception>
		public WebhookDestination(Uri url) {
			if (url == null)
				throw new ArgumentNullException(nameof(url));

			if (!url.IsAbsoluteUri)
				throw new ArgumentException($"The '{nameof(url)}' must be an absolute URI.", nameof(url));

			Url = url;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WebhookDestination"/> class
		/// </summary>
		/// <param name="url">
		/// The absolute URL of the destination.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown when the <paramref name="url"/> is <c>null</c> or empty,
		/// or when it is not an absolute URI.
		/// </exception>
		public WebhookDestination(string url)
			: this(ParseUri(url)) {
		}

		private static Uri ParseUri(string url) {
			if (String.IsNullOrWhiteSpace(url))
				throw new ArgumentException($"The '{nameof(url)}' must be a non-empty string.", nameof(url));

			if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
				throw new ArgumentException($"The '{nameof(url)}' must be an absolute URI.", nameof(url));

			return uri;
		}

		/// <summary>
		/// Gets the absolute URL of the destination.
		/// </summary>
		public Uri Url { get; }

		/// <summary>
		/// Gets or sets the options for the verification of the destination
		/// </summary>
		public WebhookDestinationVerificationOptions? Verification { get; set; } 
			= new WebhookDestinationVerificationOptions();

		/// <summary>
		/// Gets or sets a set of additional headers to be sent with the webhook.
		/// </summary>
		public IDictionary<string, string>? Headers { get; set; } = new Dictionary<string, string>();

		/// <summary>
		/// Gets or sets the format of the webhook payload.
		/// </summary>
		public WebhookFormat? Format { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the webhook should be signed.
		/// </summary>
		public bool? Sign { get; set; }

		/// <summary>
		/// Gets or sets the options for the signature of the webhook.
		/// </summary>
		public WebhookDestinationSignatureOptions? Signature { get; set; } = new WebhookDestinationSignatureOptions();

		/// <summary>
		/// Gets or sets the options for the retry of the webhook delivery.
		/// </summary>
		public WebhookRetryOptions? Retry { get; set; } = new WebhookRetryOptions();

		/// <summary>
		/// Sets the retry options for the webhook delivery to
		/// this destination.
		/// </summary>
		/// <param name="retry">
		/// The options for the retry of the webhook delivery.
		/// </param>
		/// <returns>
		/// Returns this instance of the <see cref="WebhookDestination"/> with
		/// the retry options set.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if the <paramref name="retry"/> is <c>null</c>.
		/// </exception>
		public WebhookDestination WithRetry(WebhookRetryOptions retry) {
            Retry = retry ?? throw new ArgumentNullException(nameof(retry));
            return this;
        }

		/// <summary>
		/// Sets the retry options for the webhook delivery to
		/// </summary>
		/// <param name="retry">
		/// A function that configures the retry options for the webhook delivery.
		/// </param>
		/// <returns>
		/// Returns this instance of the <see cref="WebhookDestination"/> with
		/// the retry options set.
		/// </returns>
		public WebhookDestination WithRetry(Action<WebhookRetryOptions> retry) {
			var options = new WebhookRetryOptions();
			retry(options);

			return WithRetry(options);
		}

		/// <summary>
		/// Sets the options for the signature of the webhook.
		/// </summary>
		/// <param name="signature">
		/// The options for the signature of the webhook for the destination.
		/// </param>
		/// <returns>
		/// Returns this instance of the <see cref="WebhookDestination"/> with
		/// the signature options set.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if the <paramref name="signature"/> is <c>null</c>.
		/// </exception>
		public WebhookDestination WithSignature(WebhookDestinationSignatureOptions signature) {
            Signature = signature ?? throw new ArgumentNullException(nameof(signature));
			Sign = true;
            return this;
        }

		/// <summary>
		/// Sets the options for the signature of the webhook.
		/// </summary>
		/// <param name="signature">
		/// A function that configures the options for the signature of the webhook.
		/// </param>
		/// <returns>
		/// Returns this instance of the <see cref="WebhookDestination"/> with
		/// the signature options set.
		/// </returns>
		public WebhookDestination WithSignature(Action<WebhookDestinationSignatureOptions> signature) {
			var options = new WebhookDestinationSignatureOptions();
            signature(options);
            return WithSignature(options);
		}


		/// <summary>
		/// Sets the options for the verification of the destination.
		/// </summary>
		/// <param name="options">
		/// The options that should be used for the verification of the destination.
		/// </param>
		/// <returns>
		/// Returns this instance of the <see cref="WebhookDestination"/> with
		/// the verification options set.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if the <paramref name="options"/> is <c>null</c>.
		/// </exception>
		public WebhookDestination WithVerification(WebhookDestinationVerificationOptions options) {
			Verification = options ?? throw new ArgumentNullException(nameof(options));
            return this;
		}

		/// <summary>
		/// Sets the options for the verification of the destination.
		/// </summary>
		/// <param name="configure">
		/// The function that configures the options for the verification of the destination.
		/// </param>
		/// <returns>
		/// Returns this instance of the <see cref="WebhookDestination"/> with
		/// the verification options set.
		/// </returns>
		public WebhookDestination WithVerification(Action<WebhookDestinationVerificationOptions> configure) {
			var options = new WebhookDestinationVerificationOptions();
			configure(options);
			return WithVerification(options);
		}

		/// <summary>
		/// Merges the current settings with the default settings
		/// from the configuration of a sender service.
		/// </summary>
		/// <param name="options">
		/// The default settings from the configuration of a sender service.
		/// </param>
		/// <remarks>
		/// The current settings have precedence over the default settings.
		/// </remarks>
		/// <returns>
		/// Returns a new instance of the <see cref="WebhookDestination"/> with
		/// the default settings from the configuration of a sender service
		/// merged with the current settings.
		/// </returns>
		public WebhookDestination Merge(WebhookSenderOptions options) {
			var result = new WebhookDestination(Url) {
				Sign = Sign,
				Headers = new Dictionary<string, string>(),
				Format = Format ?? options.DefaultFormat,
			};

			if (options.DefaultHeaders != null) {
				foreach (var header in options.DefaultHeaders) {
					result.Headers.Add(header.Key, header.Value);
				}
			}

			if (Headers != null) {
				foreach (var header in Headers) {
					result.Headers[header.Key] = header.Value;
				}
			}

			if (Signature == null) {
				result.Signature = new WebhookDestinationSignatureOptions(options.Signature);
			} else {
				result.Signature = Signature.Merge(options.Signature);
			}

			if (Retry == null) {
				result.Retry = new WebhookRetryOptions(options.Retry);
			} else {
				result.Retry = Retry.Merge(options.Retry);
			}

			if (Verification != null)
				result.Verification = new WebhookDestinationVerificationOptions(Verification);

			return result;
		}
	}
}
