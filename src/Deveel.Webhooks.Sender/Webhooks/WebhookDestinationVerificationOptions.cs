// Copyright 2022-2024 Antonello Provenzano
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
	/// A set of options for the verification of a webhook receiver.
	/// </summary>
	public class WebhookDestinationVerificationOptions {
		/// <summary>
		/// Initializes a new instance of the <see cref="WebhookDestinationVerificationOptions"/> class.
		/// </summary>
		public WebhookDestinationVerificationOptions() {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WebhookDestinationVerificationOptions"/> class
		/// that copies the options from the given instance.
		/// </summary>
		/// <param name="options"></param>
		public WebhookDestinationVerificationOptions(WebhookDestinationVerificationOptions options) {
			VerificationUrl = options.VerificationUrl;
			Parameters = options.Parameters?.ToDictionary(x => x.Key, y => y.Value);
		}

		/// <summary>
		/// Gets or sets the URL of the verification endpoint.
		/// </summary>
		public Uri? VerificationUrl { get; set; }

		/// <summary>
		/// Gets or sets the parameters to be sent to the verification services.
		/// </summary>
		public IDictionary<string, object>? Parameters { get; set; } = new Dictionary<string, object>();
	}
}
