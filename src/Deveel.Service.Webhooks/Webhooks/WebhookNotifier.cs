using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace Deveel.Webhooks {
	class WebhookNotifier : DefaultWebhookNotifier {
		private readonly IWebhookDataStrategy dataStrategy;

		public WebhookNotifier(IWebhookSender sender, IWebhookSubscriptionResolver subscriptionResolver, IWebhookFilterEvaluator filterEvaluator, IWebhookDataStrategy dataStrategy) 
			: base(sender, subscriptionResolver, filterEvaluator) {
			this.dataStrategy = dataStrategy;
		}

		public WebhookNotifier(IWebhookSender sender, IWebhookSubscriptionResolver subscriptionResolver, IWebhookFilterEvaluator filterEvaluator, IWebhookDataStrategy dataStrategy, ILogger<DefaultWebhookNotifier> logger) 
			: base(sender, subscriptionResolver, filterEvaluator, logger) {
			this.dataStrategy = dataStrategy;

		}

		protected override async Task<IWebhook> CreateWebhook(IWebhookSubscription subscription, EventInfo eventInfo, CancellationToken cancellationToken) {
			var factory = dataStrategy.GetDataFactory(eventInfo.EventType);
			if (factory != null) {
				try {
					eventInfo.SetData(await factory.CreateDataAsync(eventInfo, cancellationToken));
				} catch (Exception ex) {
					Logger.LogError(ex, "Error setting the data for the event {EventType} to subscription {SubscriptionId}",
						eventInfo.EventType, subscription.SubscriptionId);
					throw;
				}
				
			}

			return await base.CreateWebhook(subscription, eventInfo, cancellationToken);
		}
	}
}
