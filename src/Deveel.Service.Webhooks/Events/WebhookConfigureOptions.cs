using System;

using Newtonsoft.Json;

namespace Deveel.Webhooks {
	public class WebhookConfigureOptions {
		public JsonSerializerSettings JsonSerializerSettings { get; set; }

		public WebhookDeliveryOptions Delivery { get; set; } = new WebhookDeliveryOptions();
	}
}
