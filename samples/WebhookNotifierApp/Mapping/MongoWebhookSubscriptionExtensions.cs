using Deveel.Webhooks.Models;

namespace Deveel.Webhooks.Mapping {
	public static class MongoWebhookSubscriptionExtensions {
		public static MongoWebhookSubscription Update(this MongoWebhookSubscription subscription, WebhookSubscriptionModel model) {
			if (!String.IsNullOrWhiteSpace(model.Url))
				subscription.DestinationUrl = model.Url;
			if (!String.IsNullOrWhiteSpace(model.Secret))
				subscription.Secret = model.Secret;
			if (model.Events != null)
				subscription.EventTypes = model.Events.ToList();
			if (model.Headers != null)
				subscription.Headers = model.Headers;

			return subscription;
		}
	}
}
