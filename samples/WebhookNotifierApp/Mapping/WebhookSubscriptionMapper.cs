using Deveel.Webhooks.Models;

using Riok.Mapperly.Abstractions;

namespace Deveel.Webhooks.Mapping {
	[Mapper]
	public static partial class WebhookSubscriptionMapper {
		[MapProperty(nameof(MongoWebhookSubscription.DestinationUrl), nameof(WebhookSubscriptionModel.Url))]
		[MapProperty(nameof(MongoWebhookSubscription.EventTypes), nameof(WebhookSubscriptionModel.Events))]
		public static partial WebhookSubscriptionModel AsModel(this MongoWebhookSubscription subscription);

		[MapProperty(nameof(WebhookSubscriptionModel.Url), nameof(MongoWebhookSubscription.DestinationUrl))]
		[MapProperty(nameof(WebhookSubscriptionModel.Events), nameof(MongoWebhookSubscription.EventTypes))]
		public static partial MongoWebhookSubscription AsEntity(this WebhookSubscriptionModel model);
	}
}
