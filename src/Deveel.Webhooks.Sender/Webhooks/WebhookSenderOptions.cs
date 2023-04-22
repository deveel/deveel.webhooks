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
    /// Provides configurations for the webhooks sender service.
    /// </summary>
    public class WebhookSenderOptions {
		/// <summary>
		/// Gets or sets the name of the HTTP client registered in the
		/// factory pool and that will be used to send the webhooks. 
		/// When this is not provided, the default HTTP client is used.
		/// </summary>
		public string? HttpClientName { get; set; }

		/// <summary>
		/// Gets or sets the default headers to be sent with the webhook,
		/// additionally to the ones specified in the webhook definition.
		/// Any header with the same name of one in the webhook definition
		/// will override the value of the one in the default headers.
		/// </summary>
		public IDictionary<string, string>? DefaultHeaders { get; set; } 
			= new Dictionary<string, string>();

		/// <summary>
		/// Gets or sets the default format to be used when sending a webhook,
		/// used when the destination does not specify a format to be used
		/// (by default set to <see cref="WebhookFormat.Json"/>).
		/// </summary>
		public WebhookFormat DefaultFormat { get; set; } = WebhookFormat.Json;

		/// <summary>
		/// Gets or sets the options for the retry policy to be used
		/// when sending a webhook fails. Any specification in the
		/// <see cref="WebhookDestination.Retry"/> will override the
		/// configurations provided here.
		/// </summary>
		public WebhookRetryOptions Retry { get; set; } = new WebhookRetryOptions();

		/// <summary>
		/// Gets or sets the timeout for the HTTP request to be sent,
		/// across all retries. When this is not set, the sender has no
		/// timeout to wait for the response.
		/// </summary>
		public TimeSpan? Timeout { get; set; }

		/// <summary>
		/// Gets or sets the options for the signature of the webhooks. Any
		/// specification in the <see cref="WebhookDestination.Signature"/>
		/// will override the value of these configurations.
		/// </summary>
		public WebhookSenderSignatureOptions Signature { get; set; } 
			= new WebhookSenderSignatureOptions();
	}
}