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
	/// Provides configurations for the signature webhooks sent
	/// by the sender to receivers.
	/// </summary>
    public class WebhookSenderSignatureOptions {
		/// <summary>
		/// Gets or sets the location of the signature in the request.
		/// If this is not set, the signature is not sent.
		/// </summary>
		public WebhookSignatureLocation? Location { get; set; }

		/// <summary>
		/// Gets or sets the name of the query parameter that contains
		/// the name of the algorithm used to sign the request. This
		/// is used only when the <see cref="Location"/> is set to
		/// <see cref="WebhookSignatureLocation.QueryString"/>
		/// (default is <c>sig_alg</c>).
		/// </summary>
		public string? AlgorithmQueryParameter { get; set; } = "sig_alg";

		/// <summary>
		/// Gets or sets the name of the header that contains the
		/// signature of the webhook (default is <c>X-Webhook-Signature</c>).
		/// </summary>
		public string? HeaderName { get; set; } = "X-Webhook-Signature";

		/// <summary>
		/// Gets or sets the name of the algorithm used to sign the
		/// webhook (default is <c>sha256</c>).
		/// </summary>
		public string? Algorithm { get; set; } = "sha256";

		/// <summary>
		/// Gets or sets the name of the query parameter that contains
		/// the signature of the webhook (default is <c>sig</c>).
		/// </summary>
		public string? QueryParameter { get; set; } = "sig";
	}
}
