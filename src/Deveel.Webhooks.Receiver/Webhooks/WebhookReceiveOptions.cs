using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Deveel.Webhooks {
	public class WebhookReceiveOptions {
		public string Secret { get; set; }

		public bool ValidateSignature { get; set; } = false;

		public string SignatureHeaderName { get; set; } = "X-WEBHOOK-SIGNATURE";

		public string SignatureQueryStringKey { get; set; } = "webhook-signature";

		public WebhookSignatureLocation SignatureLocation { get; set; } = WebhookSignatureLocation.QueryString;

		public JsonSerializerSettings JsonSerializerSettings { get; set; } = new JsonSerializerSettings();
	}
}
