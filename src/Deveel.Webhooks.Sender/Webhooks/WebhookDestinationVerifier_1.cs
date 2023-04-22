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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	/// <summary>
	/// An implementation of a <see cref="IWebhookDestinationVerifier"/> that
	/// is strongly-typed to a specific webhook type.
	/// </summary>
	/// <typeparam name="TWebhook">
	/// The type of the webhook that this verifier is used for.
	/// </typeparam>
	public class WebhookDestinationVerifier<TWebhook> : WebhookDestinationVerifier, IWebhookDestinationVerifier<TWebhook>
		where TWebhook : class {
		/// <summary>
		/// Creates a new instance of the <see cref="WebhookDestinationVerifier{TWebhook}"/> class
		/// </summary>
		/// <param name="options">
		/// The options to configure the verifier service.
		/// </param>
		/// <param name="httpClientFactory">
		/// The factory of HTTP clients used to send the verification requests.
		/// </param>
		public WebhookDestinationVerifier(IOptionsSnapshot<WebhookDestinationVerifierOptions> options, 
			IHttpClientFactory? httpClientFactory = null)
			: base(options.Get(typeof(TWebhook).Name), httpClientFactory) {
		}
	}
}
