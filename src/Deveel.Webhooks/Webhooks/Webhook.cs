using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public class Webhook : IWebhook {
		public string Name { get; set; }

		public string EventType { get; set; }

		public string DestinationUrl { get; set; }

		public string Secret { get; set; }

		public IDictionary<string, string> Headers { get; set; }

		public string Id { get; set; }

		public DateTimeOffset TimeStamp { get; set; }

		public object Data { get; set; }

		public string SubscriptionId { get; set; }

		public string Format { get; set; }
	}
}
