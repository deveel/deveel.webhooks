using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Deveel.Webhooks {
	public sealed class NewtonsoftWebhookJsonParser<TWebhook> : IWebhookJsonParser<TWebhook> {
		public NewtonsoftWebhookJsonParser(JsonSerializerSettings settings = null) {
			JsonSerializerSettings = settings ?? new JsonSerializerSettings();
		}

		public JsonSerializerSettings JsonSerializerSettings { get; }

		public async Task<TWebhook> ParseWebhookAsync(Stream utf8Stream, CancellationToken cancellationToken = default) {
			try {
				using var textReader = new StreamReader(utf8Stream, Encoding.UTF8);
				var json = await textReader.ReadToEndAsync();

				return JsonConvert.DeserializeObject<TWebhook>(json, JsonSerializerSettings);
			} catch (Exception ex) {
				throw new WebhookParseException("Could not parse the stream to a webhook", ex);
			}
		}
	}
}
