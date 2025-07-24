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

using System.Text;
using System.Xml;
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
		private readonly XmlSerializerOptions options;

		/// <summary>
		/// Constructs the serializer with the given options.
		/// </summary>
		/// <param name="options">
		/// The options to use to serialize the webhook.
		/// </param>
		public SystemWebhookXmlSerializer(XmlSerializerOptions? options = null) {
			this.options = options ?? new XmlSerializerOptions();
		}

		/// <inheritdoc/>
		public async Task SerializeAsync(Stream utf8Stream, TWebhook webhook, CancellationToken cancellationToken = default) {
			ArgumentNullException.ThrowIfNull(utf8Stream, nameof(utf8Stream));

			if (!utf8Stream.CanWrite)
				throw new ArgumentException("The stream is not writable", nameof(utf8Stream));

			try {
				var settings = new XmlWriterSettings {
					Async = true,
					Indent = options.Indent,
					Encoding = Encoding.GetEncoding(options.Encoding),
					NamespaceHandling = NamespaceHandling.OmitDuplicates,
					OmitXmlDeclaration = !(options.IncludeXmlDeclaration ?? true),
					CloseOutput = false
				};

				using var writer = XmlWriter.Create(utf8Stream, settings);

				var ns = new XmlSerializerNamespaces();
				if (options.IncludeNamespaces ?? false) {
					foreach (var pair in options.Namespaces) {
						ns.Add(pair.Key, pair.Value);
					}
				} else {
					ns.Add("", "");
				}

				var serializer = new XmlSerializer(typeof(TWebhook));
				serializer.Serialize(writer, webhook, ns);
				await writer.FlushAsync();

			} catch (Exception ex) {
				throw new WebhookSerializationException("Could not serialize the webhook to XML", ex);
			}
		}
	}
}
