// Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Deveel.Webhooks.Model {
	public class TestWebhook {
		public string Id { get; set; }

		public string Event { get; set; }

		[JsonConverter(typeof(UnixDateTimeConverter))]
		public DateTimeOffset TimeStamp { get; set; }
	}
}
