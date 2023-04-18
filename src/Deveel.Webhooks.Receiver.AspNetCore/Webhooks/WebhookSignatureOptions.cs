namespace Deveel.Webhooks {
	public class WebhookSignatureOptions {
		public WebhookSignatureLocation Location { get; set; } = WebhookSignatureLocation.Header;

		public string ParameterName { get; set; }

		public string Algorithm { get; set; } = "SHA-256";

		public string Secret { get; set; }

		public int? InvalidStatusCode { get; set; } = 400;
	}
}
