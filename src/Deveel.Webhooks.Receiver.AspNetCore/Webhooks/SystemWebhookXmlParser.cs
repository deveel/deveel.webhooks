using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Deveel.Webhooks {
	public sealed class SystemWebhookXmlParser<TWebhook> : IWebhookXmlParser<TWebhook> where TWebhook : class {
		public Task<TWebhook> ParseWebhookAsync(Stream utf8Stream, CancellationToken cancellationToken = default) {
			try {
				using var textReader = new StreamReader(utf8Stream, Encoding.UTF8);
				using var xmlReader = XmlReader.Create(textReader);
				var serializer = new XmlSerializer(typeof(TWebhook));

				if (!serializer.CanDeserialize(xmlReader))
					throw new WebhookParseException("Cannot deserialize the XML content");

				var result = serializer.Deserialize(xmlReader) as TWebhook;
				if (result == null)
					throw new WebhookParseException("Cannot deserialize the XML content");

				return Task.FromResult(result);
			} catch (WebhookParseException) {
				throw;
			} catch(Exception ex) {
				throw new WebhookParseException("Unable to deserialize the XML content", ex);
			}
		}
	}
}
