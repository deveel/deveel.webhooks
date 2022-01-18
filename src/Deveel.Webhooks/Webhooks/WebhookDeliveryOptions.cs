using System;

using Newtonsoft.Json;

namespace Deveel.Webhooks {
	public class WebhookDeliveryOptions {
		public string SignatureHeaderName { get; set; } = "X-WEBHOOK-SIGNATURE";

		public string SignatureQueryStringKey { get; set; } = "webhook-signature";

		public bool SignWebhooks { get; set; } = true;

		public WebhookFields IncludeFields { get; set; } = WebhookFields.All;

		public WebhookSignatureLocation SignatureLocation { get; set; } = WebhookSignatureLocation.QueryString;

		public string SignatureAlgorithm { get; set; } = WebhookSignatureAlgorithms.HmacSha256;

		public int MaxAttemptCount { get; set; } = 3;

		public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(2);

		public JsonSerializerSettings JsonSerializerSettings { get; set; } = new JsonSerializerSettings();
	}
}
