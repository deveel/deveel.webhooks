using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Deveel.Webhooks {
	class WebhookPayload {
		[JsonProperty("webhook")]

		public string WebhookName { get; set; }

		[JsonProperty("event_id")]
		public string EventId { get; set; }

		[JsonProperty("event_name")]
		public string EventType { get; set; }


		[JsonExtensionData]
		public JObject Data { get; set; }
	}
}
