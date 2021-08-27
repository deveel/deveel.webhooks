using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Deveel.Webhooks {
	public class WebhookPayload {
		[JsonProperty("webhook")]
		public string WebhookName { get; set; }

		[JsonProperty("eventId")]
		public string EventId { get; set; }

		[JsonProperty("eventType")]
		public string EventType { get; set; }

		[JsonProperty("timeStamp")]
		public DateTimeOffset TimeStamp { get; set; }

		[JsonExtensionData]
		public JObject Data { get; set; }
	}
}
