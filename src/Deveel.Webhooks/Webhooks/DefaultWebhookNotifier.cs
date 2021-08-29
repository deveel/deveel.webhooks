﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Deveel.Webhooks {
	public class DefaultWebhookNotifier : IWebhookNotifier {
		private readonly IWebhookSubscriptionResolver subscriptionResolver;
		private readonly IWebhookFilterRequestFactory requestFactory;
		private readonly IWebhookFilterEvaluator filterEvaluator;
		private readonly IWebhookSender sender;

		public DefaultWebhookNotifier(IWebhookSender sender,
			IWebhookSubscriptionResolver subscriptionResolver,
			IWebhookFilterRequestFactory requestFactory,
			IWebhookFilterEvaluator filterEvaluator,
			ILogger<DefaultWebhookNotifier> logger) {
			this.sender = sender;
			this.requestFactory = requestFactory;
			this.subscriptionResolver = subscriptionResolver;
			this.filterEvaluator = filterEvaluator;
			Logger = logger;
		}

		public DefaultWebhookNotifier(IWebhookSender sender,
			IWebhookSubscriptionResolver subscriptionResolver,
			IWebhookFilterEvaluator filterEvaluator,
			ILogger<DefaultWebhookNotifier> logger)
			: this(sender, subscriptionResolver, new DefaultWebookFilterRequestFactory(), filterEvaluator, logger) {
		}

		public DefaultWebhookNotifier(IWebhookSender sender, IWebhookSubscriptionResolver subscriptionResolver, IWebhookFilterRequestFactory requestFactory, IWebhookFilterEvaluator filterEvaluator)
			: this(sender, subscriptionResolver, requestFactory, filterEvaluator, NullLogger<DefaultWebhookNotifier>.Instance) {
		}

		public DefaultWebhookNotifier(IWebhookSender sender, IWebhookSubscriptionResolver subscriptionResolver, IWebhookFilterEvaluator filterEvaluator)
			: this(sender, subscriptionResolver, new DefaultWebookFilterRequestFactory(), filterEvaluator, NullLogger<DefaultWebhookNotifier>.Instance) {
		}


		protected ILogger Logger { get; }

		protected virtual WebhookFilterRequest BuildFilterRequest(IWebhookSubscription subscription) {
			if (requestFactory != null)
				return requestFactory.CreateRequest(subscription);

			return WebhookFilterRequest.Empty;
		}

		protected virtual async Task<bool> MatchesAsync(WebhookFilterRequest filterRequest, IWebhook webhook, CancellationToken cancellationToken) {
			if (filterRequest == null)
				return true;

			return await filterEvaluator.MatchesAsync(filterRequest, webhook, cancellationToken);
		}

		public async Task<WebhookNotificationResult> NotifyAsync(string tenantId, EventInfo eventInfo, CancellationToken cancellationToken) {
			var result = new WebhookNotificationResult();

			try {
				var subscriptions = await subscriptionResolver.ResolveSubscriptionsAsync(tenantId, eventInfo.EventType, cancellationToken);

				if (subscriptions == null || subscriptions.Count == 0) {
					Logger.LogInformation("No subscriptions to event {EventType} found for Tenant {TenantId}", eventInfo.EventType, tenantId);
					return result;
				}

				foreach (var subscription in subscriptions) {
					var webhook = await CreateWebhook(subscription, eventInfo, cancellationToken);

					try {
						var filterRequest = BuildFilterRequest(subscription);

						if (await MatchesAsync(filterRequest, webhook, cancellationToken)) {
							Logger.LogInformation("Delivering webhook for event {EventType} to subscription {SubscriptionId} of Tenant {TenantId}",
								eventInfo.EventType, subscription.SubscriptionId, tenantId);

							var deliveryResult = await SendAsync(tenantId, webhook, cancellationToken);

							result.AddDelivery(deliveryResult);
						} else {
							Logger.LogInformation("Not delivering the webhook for event {EventType} to subscription {SubscriptionId} of Tenant {TenantId}",
								eventInfo.EventType, subscription.SubscriptionId, tenantId);
						}
					} catch (Exception ex) {
						Logger.LogError(ex, "Could not deliver a webhook for event {EventType} to subscription {SubscriptionId} of Tenant {TenantId}",
							eventInfo.EventType, subscription.SubscriptionId, tenantId);

						result.AddDelivery(new WebhookDeliveryResult(webhook));
					}
				}

				return result;
			} catch (Exception ex) {
				Logger.LogError(ex, "Could not notify the event {EventType} to tenant {TenantId}", eventInfo.EventType, tenantId);
				throw;
			}
		}

		protected virtual Task<WebhookDeliveryResult> SendAsync(string tenantId, IWebhook webhook, CancellationToken cancellationToken) {
			try {
				return sender.SendAsync(webhook, cancellationToken);
			} catch (Exception ex) {
				Logger.LogError(ex, "The webhook sender failed to send a webhook for event {EventType} to tenant {TenantId} because of an error",
					webhook.EventType, tenantId);
				throw;
			}
		}

		protected virtual Task<IWebhook> CreateWebhook(IWebhookSubscription subscription, EventInfo eventInfo, CancellationToken cancellationToken) {
			var webhook = new Webhook {
				SubscriptionId = subscription.SubscriptionId,
				Name = subscription.Name,
				EventType = eventInfo.EventType,
				DestinationUrl = subscription.DestinationUrl,
				Headers = subscription.Headers == null
					? null
					: new Dictionary<string, string>(subscription.Headers),
				Data = eventInfo.Data,
				Secret = subscription.Secret,
				Id = eventInfo.Id,
				TimeStamp = eventInfo.TimeStamp
			};

			return Task.FromResult<IWebhook>(webhook);
		}

		#region Webhook

		class Webhook : IWebhook {
			public string Name { get; set; }

			public string EventType { get; set; }

			public string DestinationUrl { get; set; }

			public string Secret { get; set; }

			public IDictionary<string, string> Headers { get; set; }

			public string Id { get; set; }

			public DateTimeOffset TimeStamp { get; set; }

			public object Data { get; set; }

			public string SubscriptionId { get; set; }
		}

		#endregion
	}
}
