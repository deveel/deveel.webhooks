using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public sealed class DefaultWehbookFactory : IWebhookFactory<Webhook> {
		public Task<Webhook> CreateAsync(IWebhookSubscription subscription, EventInfo eventInfo, CancellationToken cancellationToken) {
			var webhook = new Webhook {
				Id = eventInfo.Id,
				EventType = eventInfo.EventType,
				SubscriptionId = subscription.SubscriptionId,
				DestinationUrl = subscription.DestinationUrl,
				TimeStamp = eventInfo.TimeStamp,
				Headers = new Dictionary<string, string>(),
				Secret = subscription.Secret,
				Data = eventInfo.Data,
			};
			if (subscription.Headers != null) {
				foreach (var header in subscription.Headers) {
					webhook.Headers[header.Key] = header.Value;
				}
			}

			return Task.FromResult(webhook);
		}
	}
}
