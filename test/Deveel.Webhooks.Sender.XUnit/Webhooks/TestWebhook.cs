using System;

namespace Deveel.Webhooks {
	public class TestWebhook {
		public string Id { get; set; }

		public string Event { get; set; }

		public DateTimeOffset TimeStamp { get; set; }
	}
}
