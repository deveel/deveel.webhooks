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
	/// <summary>
	/// Provides the configuration settings used to verify the signature
	/// of a webhook sent to the receiver.
	/// </summary>
	public class WebhookSignatureOptions {
		/// <summary>
		/// Gets or sets the location where the signature is found (<see cref="WebhookSignatureLocation.Header"/> by default).
		/// </summary>
		public WebhookSignatureLocation Location { get; set; } = WebhookSignatureLocation.Header;

		/// <summary>
		/// Gets or sets the name of the parameter that contains the signature.
		/// </summary>
		public string? ParameterName { get; set; }

		/// <summary>
		/// Gets or sets the type of algorithm used to compute the signature.
		/// </summary>
		public string? Algorithm { get; set; }

		/// <summary>
		/// Gets or sets the name of the query parameter that contains the algorithm,
		/// when the signature is sent in a query string.
		/// </summary>
		public string? AlgorithmQueryParameter { get; set; } = "sig_alg";

		/// <summary>
		/// Gets or sets the name of the header that contains the algorithm,
		/// when the signature is sent in a header.
		/// </summary>
		public string? AlgorithmHeaderName { get; set; } = "X-Signature-Algorithm";

		/// <summary>
		/// Gets or sets the secret used to compute the signature.
		/// </summary>
		public string? Secret { get; set; }

		/// <summary>
		/// Gets or sets the HTTP status code to return when the webhook
		/// signature is invalid (<c>400</c> by default).
		/// </summary>
		public int? InvalidStatusCode { get; set; } = 400;

		/// <summary>
		/// Gets or sets the <see cref="IWebhookSigner"/> to use
		/// for verifying the signature of the webhooks.
		/// </summary>
		public IWebhookSigner? Signer {
			get => Signers.Count > 0 ? Signers[0] : null;
			set => Signers = (value != null ? new List<IWebhookSigner> { value } : new List<IWebhookSigner>());
		}

		/// <summary>
		/// Gets or sets a list of <see cref="IWebhookSigner"/> that are supported
		/// by the receiver for validating the signature of the webhooks.
		/// </summary>
		public IList<IWebhookSigner> Signers { get; set; } = new List<IWebhookSigner>();
	}
}
