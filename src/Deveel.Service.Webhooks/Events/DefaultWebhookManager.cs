using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Data;
using Deveel.Events;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Deveel.Webhooks {
	public class DefaultWebhookManager : IWebhookManager {
		private readonly IWebhookSubscriptionFactory subscriptionFactory;
		private readonly IWebhookSubscriptionStoreProvider subscriptionStore;
		private readonly IWebhookDataStrategy dataStrategy;
		private readonly IWebhookSender webhookSender;
		private readonly ILogger logger;

		public DefaultWebhookManager(IWebhookSubscriptionStoreProvider subscriptionStore,
			IWebhookSubscriptionFactory subscriptionFactory,
			IWebhookDataStrategy dataStrategy, IWebhookSender webhookSender)
			: this(subscriptionStore, subscriptionFactory, dataStrategy, webhookSender, NullLogger<DefaultWebhookManager>.Instance) {
		}

		public DefaultWebhookManager(IWebhookSubscriptionStoreProvider subscriptionStore,
			IWebhookSubscriptionFactory subscriptionFactory,
			IWebhookDataStrategy dataStrategy, IWebhookSender webhookSender, ILogger<DefaultWebhookManager> logger) {
			this.subscriptionStore = subscriptionStore;
			this.subscriptionFactory = subscriptionFactory;
			this.dataStrategy = dataStrategy;
			this.webhookSender = webhookSender;
			this.logger = logger;
		}

		public DefaultWebhookManager(IWebhookSubscriptionStoreProvider subscriptionStore,
			IWebhookSubscriptionFactory subscriptionConverter,
			IWebhookSender webhookSender)
			: this(subscriptionStore, subscriptionConverter, null, webhookSender) {
		}


		public virtual async Task<string> AddSubscriptionAsync(string tenantId, WebhookSubscriptionInfo subscriptionInfo, CancellationToken cancellationToken) {
			var subscription = subscriptionFactory.CreateSubscription(subscriptionInfo);

			try {
				var result = await subscriptionStore.CreateAsync(tenantId, subscription, cancellationToken);

				logger.LogInformation("New subscription with ID {SubscriptionId} for Tenant {TenantId}", result, tenantId);

				return result;
			} catch (Exception ex) {
				logger.LogError(ex, "Error while creating a subscription for Tenant {TenantId}", tenantId);
				throw;
			}
		}

		public virtual async Task<bool> RemoveSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken) {
			try {
				var subscription = await subscriptionStore.FindByIdAsync(tenantId, subscriptionId, cancellationToken);

				if (subscription == null) {
					logger.LogWarning("Trying to delete the subscription {SubscriptionId} of Tenant {TenantId}, but it was not found",
						subscriptionId, tenantId);
					return false;
				}

				var result = await subscriptionStore.DeleteAsync(tenantId, subscription);
				if (!result) {
					logger.LogWarning("The subscription {SubscriptionId} of Tenant {TenantId} was not deleted from the store",
						subscriptionId, tenantId);
				} else {
					logger.LogInformation("The subscription {SubscriptionId} of Tenant {TenantId} was deleted from the store");
				}

				return result;
			} catch (Exception ex) {
				logger.LogError(ex, "Error while delete subscription {SubscriptionId} of Tenant {TenantId}", subscriptionId, tenantId);
				throw;
			}
		}

		private static WebhookSubscriptionInfo Convert(IWebhookSubscription subscription) {
			if (subscription == null)
				return null;

			return new WebhookSubscriptionInfo(subscription.EventType, subscription.DestinationUrl) {
				SubscriptionId = subscription.Id,
				Name = subscription.Name,
				Headers = subscription.Headers != null
					? new Dictionary<string, string>(subscription.Headers)
					: new Dictionary<string, string>(),
				FilterExpressions = subscription.FilterExpressions?.ToList(),
				Metadata = subscription.Metadata == null
					? new Dictionary<string, object>()
					: new Dictionary<string, object>(subscription.Metadata),
				RetryCount = subscription.RetryCount,
				Secret = subscription.Secret
			};
		}

		public virtual async Task<WebhookSubscriptionInfo> GetSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken)
			=> Convert(await subscriptionStore.FindByIdAsync(tenantId, subscriptionId, cancellationToken));

		public async Task<PaginatedResult<WebhookSubscriptionInfo>> GetSubscriptionsAsync(string tenantId, PageRequest page, CancellationToken cancellationToken) {
			var pageResult = await subscriptionStore.GetPageAsync(tenantId, page, cancellationToken);
			return pageResult.Cast(Convert);
		}

		public async Task<PaginatedResult<WebhookSubscriptionInfo>> GetSubscriptionsByMetadataAsync(string tenantId, string key, object value, PageRequest page, CancellationToken cancellationToken) {
			var result = await subscriptionStore.GetPageByMetadataAsync(tenantId, key, value, page, cancellationToken);
			return result.Cast(Convert);
		}

		private async Task<bool> SetStateAsync(string tenantId, string subscriptionId, bool active, CancellationToken cancellationToken) {
			var subscription = await subscriptionStore.FindByIdAsync(tenantId, subscriptionId, cancellationToken);
			if (subscription == null ||
				subscription.IsActive == active)
				return false;

			await subscriptionStore.SetStateAsync(tenantId, subscription, active, cancellationToken);
			await subscriptionStore.UpdateAsync(tenantId, subscription);

			return true;
		}

		public virtual Task<bool> EnableSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken)
			=> SetStateAsync(tenantId, subscriptionId, true, cancellationToken);

		public virtual Task<bool> DisableSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken)
			=> SetStateAsync(tenantId, subscriptionId, false, cancellationToken);

		public virtual async Task<WebhookNotificationResult> NotifyAsync(string tenantId, WebhookNotification notification, CancellationToken cancellationToken) {
			var subscriptions = await subscriptionStore.GetAllAsync(tenantId, 20, cancellationToken);

			if (subscriptions == null || subscriptions.Count == 0) {
				logger.LogInformation("No subscriptions to event {EventType} found for Tenant {TenantId}", notification.Type, tenantId);
				return new WebhookNotificationResult();
			}

			var deliveryResults = new Dictionary<string, WebhookDeliveryResult>();

			foreach (var subscription in subscriptions) {
				var webhook = await CreateWebhook(subscription, notification, cancellationToken);

				try {
					if (subscription.Matches(webhook)) {
						logger.LogInformation("Delivering webhook for event {EventType} to subscription {SubscriptionId} of Tenant {TenantId}",
							notification.Type, subscription.Id, tenantId);

						var result = await SendAsync(tenantId, webhook, cancellationToken);

						deliveryResults[subscription.Id] = result;
					}
				} catch (Exception ex) {
					logger.LogError(ex, "Could not deliver a webhook for event {EventType} to subscription {SubscriptionId} of Tenant {TenantId}",
						notification.Type, subscription.Id, tenantId);

					deliveryResults[subscription.Id] = new WebhookDeliveryResult(webhook);
				}
			}

			return new WebhookNotificationResult(deliveryResults);
		}

		protected virtual Task<WebhookDeliveryResult> SendAsync(string tenantId, IWebhook webhook, CancellationToken cancellationToken) {
			try {
				return webhookSender.SendAsync(webhook, cancellationToken);
			} catch (Exception ex) {
				logger.LogError(ex, "The webhook sender failed to send because of an error");
				throw;
			}
		}

		private async Task<IWebhook> CreateWebhook(IWebhookSubscription subscription, IEvent @event, CancellationToken cancellationToken) {
			var dataProvider = dataStrategy?.GetDataFactory(@event.Type);
			var data = @event.Data;

			if (dataProvider != null)
				data = await dataProvider.CreateDataAsync(@event, cancellationToken);

			return new Webhook {
				SubscriptionId = subscription.Id,
				Name = subscription.Name,
				Type = @event.Type,
				DestinationUrl = subscription.DestinationUrl,
				Headers = subscription.Headers == null ? null : new Dictionary<string, string>(subscription.Headers),
				Data = data,
				Secret = subscription.Secret,
				Id = @event.Id,
				TimeStamp = @event.TimeStamp
			};
		}

		#region Webhook

		class Webhook : IWebhook {
			public string Name { get; set; }

			public string DestinationUrl { get; set; }

			public string Secret { get; set; }

			public IDictionary<string, string> Headers { get; set; }

			public string Id { get; set; }

			public string Type { get; set; }

			public DateTimeOffset TimeStamp { get; set; }

			public object Data { get; set; }

			public string DataVersion { get; set; }

			public string SubscriptionId { get; set; }
		}

		#endregion
	}
}
