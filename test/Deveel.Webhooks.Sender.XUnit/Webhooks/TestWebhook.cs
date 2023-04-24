#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using System;

namespace Deveel.Webhooks {
	public class TestWebhook {
		public string Id { get; set; }

		public string Event { get; set; }

		public DateTimeOffset TimeStamp { get; set; }
	}
}
