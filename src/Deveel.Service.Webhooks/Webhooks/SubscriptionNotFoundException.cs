using System;
using System.Collections.Generic;
using System.Text;

namespace Deveel.Webhooks {
	public sealed class SubscriptionNotFoundException : Exception {
		public SubscriptionNotFoundException(string subscriptionId) : this(subscriptionId, $"Subscription {subscriptionId} was not found") {
		}

		public SubscriptionNotFoundException(string subscriptionId, string message) : base(message) {
			SubscriptionId = subscriptionId;
		}

		public string SubscriptionId { get; }
	}
}
