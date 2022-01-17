using System;

namespace Deveel.Webhooks {
	public sealed class WebhookSubscriptionStateInfo {
		public WebhookSubscriptionStateInfo(WebhookSubscriptionStatus status, string userId = null) {
			Status = status;
			TimeStamp = DateTime.UtcNow;
			UserId = userId;
		}

		public string UserId { get; set; }

		public WebhookSubscriptionStatus Status { get; }

		public DateTimeOffset TimeStamp { get; set; }
	}
}
