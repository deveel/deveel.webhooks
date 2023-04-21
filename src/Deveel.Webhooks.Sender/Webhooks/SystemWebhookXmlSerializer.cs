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
using System.Xml.Serialization;

namespace Deveel.Webhooks {
	/// <summary>
	/// A default implementation of <see cref="IWebhookXmlSerializer{TWebhook}"/> that
	/// uses the built-in <see cref="XmlSerializer"/> to serialize the webhook.
	/// </summary>
	/// <typeparam name="TWebhook">
	/// The type of the webhook to serialize.
	/// </typeparam>
	public sealed class SystemWebhookXmlSerializer<TWebhook> : IWebhookXmlSerializer<TWebhook> where TWebhook : class {
		/// <inheritdoc/>
		public async Task SerializeWebhookAsync(Stream utf8Stream, TWebhook webhook, CancellationToken cancellationToken = default) {
			try {
				var serializer = new XmlSerializer(typeof(TWebhook));
				serializer.Serialize(utf8Stream, webhook);

				await Task.CompletedTask;
			} catch (Exception ex) {
				throw new WebhookSerializationException("Could not serialize the webhook to XML", ex);
			}
		}
	}
}
