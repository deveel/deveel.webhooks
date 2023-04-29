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

using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Deveel.Webhooks {
    /// <summary>
    /// The default implementation of <see cref="IWebhookXmlParser{TWebhook}"/> that
    /// uses the <c>System.Xml</c> components to parse incoming webhooks.
    /// </summary>
    /// <typeparam name="TWebhook">
	/// The type of webhook that is parsed by the parser.
	/// </typeparam>
    public sealed class SystemWebhookXmlParser<TWebhook> : 
		IWebhookXmlParser<TWebhook>
		where TWebhook : class {
		/// <inheritdoc/>
		public Task<TWebhook?> ParseWebhookAsync(Stream utf8Stream, CancellationToken cancellationToken = default) {
			try {
				using var textReader = new StreamReader(utf8Stream, Encoding.UTF8);
				using var xmlReader = XmlReader.Create(textReader);
				var serializer = new XmlSerializer(typeof(TWebhook));

				if (!serializer.CanDeserialize(xmlReader))
					throw new WebhookParseException("Cannot deserialize the XML content");

				var result = serializer.Deserialize(xmlReader) as TWebhook;

				return Task.FromResult(result);
			} catch (WebhookParseException) {
				throw;
			} catch(Exception ex) {
				throw new WebhookParseException("Unable to deserialize the XML content", ex);
			}
		}
	}
}
