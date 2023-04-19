using System.Text.Json;

namespace Deveel.Webhooks {
	public sealed class SystemTextWebhookJsonSerializer<TWebhook> : IWebhookJsonSerializer<TWebhook>
		where TWebhook : class {
		public JsonSerializerOptions JsonSerializerOptions { get; }

		public SystemTextWebhookJsonSerializer(JsonSerializerOptions? options = null) {
			JsonSerializerOptions = options ?? new JsonSerializerOptions();
		}

		public async Task SerializeWebhookAsync(Stream utf8Stream, TWebhook webhook, CancellationToken cancellationToken) {
			try {
				await JsonSerializer.SerializeAsync(utf8Stream, webhook, JsonSerializerOptions, cancellationToken);
			} catch (Exception ex) {
				throw new WebhookSerializationException("It was not possible to serialize the webhook", ex);
			}
		}
	}
}
