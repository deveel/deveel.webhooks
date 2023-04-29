﻿using Deveel.Webhooks.Model;

namespace Deveel.Webhooks.Handlers {
	public class TestWebhookReceiver : IWebhookReceiver<TestWebhook> {
		public async Task<WebhookReceiveResult<TestWebhook>> ReceiveAsync(HttpRequest request, CancellationToken cancellationToken) {
			// TODO: test the signature as well ...

			var result = await request.ReadFromJsonAsync<TestWebhook>();
			return new WebhookReceiveResult<TestWebhook>(result);
		}
	}
}
