using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public class MongoWebhookReceiver : IWebhookReceiver {
		public string DestinationUrl { get; set; }

		IEnumerable<KeyValuePair<string, string>> IWebhookReceiver.Headers => Headers;

		public IDictionary<string, string> Headers { get; set; }

		public string BodyFormat { get; set; }

		public string SubscriptionId { get; set; }

		public string SubscriptionName { get; set; }
	}
}
