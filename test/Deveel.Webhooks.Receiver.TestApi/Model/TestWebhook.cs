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
