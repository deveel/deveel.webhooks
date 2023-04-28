using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public static class WebhookXmlParserExtensions {
		public static async Task<TWebhook?> ParseWebhookAsync<TWebhook>(this IWebhookXmlParser<TWebhook> parser, string? xmlBody, CancellationToken cancellationToken)
			where TWebhook : class {

			if (string.IsNullOrWhiteSpace(xmlBody))
				return null;

			using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlBody));
			return await parser.ParseWebhookAsync(stream, cancellationToken);
		}
	}
}
