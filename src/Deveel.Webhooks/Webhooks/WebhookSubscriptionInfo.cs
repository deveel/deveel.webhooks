using System;
using System.Collections.Generic;
using System.Linq;

using Deveel.Events;

namespace Deveel.Webhooks {
	public sealed class WebhookSubscriptionInfo : IWebhookSubscription {
		public WebhookSubscriptionInfo(string eventTypes, string destinationUrl)
			: this(new[] { eventTypes }, new Uri(destinationUrl)) {
		}

		public WebhookSubscriptionInfo(string[] eventTypes, string destinationUrl)
			: this(eventTypes, new Uri(destinationUrl)) {
		}

		public WebhookSubscriptionInfo(string eventType, Uri destinationUrl)
			: this(new [] { eventType }, destinationUrl) {
		}


		public WebhookSubscriptionInfo(string[] eventTypes, Uri destinationUrl) {
			if (eventTypes == null || eventTypes.Length == 0)
				throw new ArgumentException("At least one event type is required");
			if (eventTypes.Any(x => String.IsNullOrWhiteSpace(x)))
				throw new ArgumentException($"'{nameof(eventTypes)}' cannot contain null or whitespace entries.", nameof(eventTypes));

			EventTypes = eventTypes;
			DestinationUrl = destinationUrl;
		}

		public string SubscriptionId { get; set; }

		string IWebhookSubscription.Id => SubscriptionId;

		bool IWebhookSubscription.IsActive => true;

		public string Name { get; set; }

		public string[] EventTypes { get; }

		public Uri DestinationUrl { get; }

		string IWebhookSubscription.DestinationUrl => DestinationUrl.ToString();

		public int RetryCount { get; set; }

		public string Secret { get; set; }

		public IEnumerable<IWebhookFilter> Filters { get; set; }

		public IWebhookFilter Filter {
			get => Filters?.SingleOrDefault();
			set => Filters = new[] { value };
		}

		public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

		public IDictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
	}
}
