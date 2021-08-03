using System;
using System.Collections.Generic;

using Deveel.Events;

namespace Deveel.Webhooks {
	public sealed class WebhookSubscriptionInfo : IWebhookSubscription {
		public WebhookSubscriptionInfo(string eventType, string destinationUrl)
			: this(eventType, new Uri(destinationUrl)) {
		}

		public WebhookSubscriptionInfo(string eventType, Uri destinationUrl) {
			if (string.IsNullOrWhiteSpace(eventType))
				throw new ArgumentException($"'{nameof(eventType)}' cannot be null or whitespace.", nameof(eventType));

			EventType = eventType;
			DestinationUrl = destinationUrl;
		}

		public string SubscriptionId { get; set; }

		string IEntity.Id => SubscriptionId;

		bool IWebhookSubscription.IsActive => true;

		public string Name { get; set; }

		public string EventType { get; }

		public Uri DestinationUrl { get; }

		string IWebhookSubscription.DestinationUrl => DestinationUrl.ToString();

		public int RetryCount { get; set; }

		public string Secret { get; set; }

		public IList<string> FilterExpressions { get; set; }

		IEnumerable<string> IEventSubscription.FilterExpressions => FilterExpressions;

		public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

		public IDictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
	}
}
