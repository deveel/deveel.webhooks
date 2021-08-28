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
	public class DefaultWebhookSubscriptionManager : IWebhookSubscriptionManager {
		private readonly IWebhookSubscriptionFactory subscriptionFactory;
		private readonly IWebhookSubscriptionStoreProvider subscriptionStore;
		private readonly ILogger logger;

		public DefaultWebhookSubscriptionManager(IWebhookSubscriptionStoreProvider subscriptionStore,
			IWebhookSubscriptionFactory subscriptionFactory)
			: this(subscriptionStore, subscriptionFactory, NullLogger<DefaultWebhookSubscriptionManager>.Instance) {
		}

		public DefaultWebhookSubscriptionManager(IWebhookSubscriptionStoreProvider subscriptionStore,
			IWebhookSubscriptionFactory subscriptionFactory,
			ILogger<DefaultWebhookSubscriptionManager> logger) {
			this.subscriptionStore = subscriptionStore;
			this.subscriptionFactory = subscriptionFactory;
			this.logger = logger;
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

		public virtual async Task<IWebhookSubscription> GetSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken)
			=> await subscriptionStore.FindByIdAsync(tenantId, subscriptionId, cancellationToken);

		public async Task<PaginatedResult<IWebhookSubscription>> GetSubscriptionsAsync(string tenantId, PageRequest page, CancellationToken cancellationToken) {
			return await subscriptionStore.GetPageAsync(tenantId, page, cancellationToken);
		}

		public async Task<PaginatedResult<IWebhookSubscription>> GetSubscriptionsByMetadataAsync(string tenantId, string key, object value, PageRequest page, CancellationToken cancellationToken) {
			return await subscriptionStore.GetPageByMetadataAsync(tenantId, key, value, page, cancellationToken);
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
	}
}
