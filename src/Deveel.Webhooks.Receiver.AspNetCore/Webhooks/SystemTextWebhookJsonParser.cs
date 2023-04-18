using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public sealed class SystemTextWebhookJsonParser<TWebhook> : IWebhookJsonParser<TWebhook> where TWebhook : class {
		public SystemTextWebhookJsonParser(JsonSerializerOptions options = null) {
			JsonSerializerOptions = options ?? new JsonSerializerOptions();
		}

		public JsonSerializerOptions JsonSerializerOptions { get; }

		public async Task<TWebhook> ParseWebhookAsync(Stream utf8Stream, CancellationToken cancellationToken = default) {
			try {
				return await JsonSerializer.DeserializeAsync<TWebhook>(utf8Stream, JsonSerializerOptions, cancellationToken);
			} catch (Exception ex) {
				throw new WebhookParseException("Could not parse the stream to a webhook", ex);
			}
		}
	}
}
