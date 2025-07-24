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

using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	/// <summary>
	/// A default implementation of the <see cref="IWebhookFactory{TWebhook}"/>
	/// that creates a <see cref="Webhook"/> instance using the information
	/// provided by the subscription and the event.
	/// </summary>
	public sealed class DefaultWebhookFactory : DefaultWebhookFactory<Webhook> {
		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultWebhookFactory"/> class with the specified options.
		/// </summary>
		/// <param name="options">The configuration options for the webhook factory. Cannot be null.</param>
		public DefaultWebhookFactory(IOptions<WebhookFactoryOptions<Webhook>> options) : base(options) {
		}
	}
}
