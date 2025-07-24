// Copyright 2022-2025 Antonello Provenzano
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
	/// Provides options for the signature of the webhook
	/// specific for a destination.
	/// </summary>
	/// <remarks>
	/// This set of options inherit from the base <see cref="WebhookSenderSignatureOptions"/>
	/// and introduce some additional properties that are specific for a destination.
	/// </remarks>
	public class WebhookDestinationSignatureOptions : WebhookSenderSignatureOptions {
		/// <summary>
		/// Initializes an empty instance of the <see cref="WebhookDestinationSignatureOptions"/>.
		/// </summary>
		public WebhookDestinationSignatureOptions() {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WebhookDestinationSignatureOptions"/>
		/// that copies the options from the given parent.
		/// </summary>
		/// <param name="options">
		/// The base options to copy from.
		/// </param>
		public WebhookDestinationSignatureOptions(WebhookSenderSignatureOptions options) 
			: base(options) {
		}

		/// <summary>
		/// Gets or sets the receiver-specific secret used to sign a webhook.
		/// </summary>
		public string? Secret { get; set; }

		/// <summary>
		/// Merges this instance with the given options, returning a new
		/// set that is the result of the merge.
		/// </summary>
		/// <param name="options">
		/// The set of options to merge with this instance.
		/// </param>
		/// <returns>
		/// Returns a new instance of <see cref="WebhookDestinationSignatureOptions"/>
		/// that is the result of the merge.
		/// </returns>
		public WebhookDestinationSignatureOptions Merge(WebhookSenderSignatureOptions? options) {
			return new WebhookDestinationSignatureOptions {
				Secret = Secret,
				Algorithm = Algorithm ?? options?.Algorithm,
				HeaderName = HeaderName ?? options?.HeaderName,
				Location = Location ?? options?.Location,
				QueryParameter = QueryParameter ?? options?.QueryParameter,
				AlgorithmQueryParameter = AlgorithmQueryParameter ?? options?.AlgorithmQueryParameter
			};
		}
	}
}
