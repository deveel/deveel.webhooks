using System;

namespace Deveel.Webhooks {
	public sealed class SubscriptionNotFoundException : WebhookServiceException {
		public SubscriptionNotFoundException(string subscriptionId) : this(subscriptionId, $"Subscription {subscriptionId} was not found") {
		}

		public SubscriptionNotFoundException(string subscriptionId, string message) : base(message) {
			SubscriptionId = subscriptionId;
		}

		public string SubscriptionId { get; }
	}
}
