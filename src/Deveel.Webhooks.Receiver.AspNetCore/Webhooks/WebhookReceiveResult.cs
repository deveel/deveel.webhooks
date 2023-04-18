namespace Deveel.Webhooks {
	public readonly struct WebhookReceiveResult<TWebhook> where TWebhook : class {
		public WebhookReceiveResult(TWebhook webhook, bool? signatureValid) : this() {
			Webhook = webhook;
			SignatureValid = signatureValid;
		}

		public TWebhook Webhook { get; }

		public bool? SignatureValid { get; }

		public static implicit operator WebhookReceiveResult<TWebhook>(TWebhook webhook)
			=> new WebhookReceiveResult<TWebhook>(webhook, null);
	}
}
