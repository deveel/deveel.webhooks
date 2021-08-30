﻿using System;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Data;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Deveel.Webhooks {
	public class DefaultWebhookSubscriptionManager<TSubscription> : IWebhookSubscriptionManager<TSubscription>
		where TSubscription : class, IWebhookSubscription {
		private readonly IWebhookSubscriptionFactory<TSubscription> subscriptionFactory;
		private readonly IWebhookSubscriptionStoreProvider<TSubscription> subscriptionStore;

		protected DefaultWebhookSubscriptionManager(IWebhookSubscriptionStoreProvider<TSubscription> subscriptionStore,
			IWebhookSubscriptionFactory<TSubscription> subscriptionFactory,
			ILogger logger) {
			this.subscriptionStore = subscriptionStore;
			this.subscriptionFactory = subscriptionFactory;
			Logger = logger;
		}

		public DefaultWebhookSubscriptionManager(IWebhookSubscriptionStoreProvider<TSubscription> subscriptionStore,
			IWebhookSubscriptionFactory<TSubscription> subscriptionFactory,
			ILogger<DefaultWebhookSubscriptionManager<TSubscription>> logger)
			: this(subscriptionStore, subscriptionFactory, (ILogger)logger) {
		}

		public DefaultWebhookSubscriptionManager(IWebhookSubscriptionStoreProvider<TSubscription> subscriptionStore,
			IWebhookSubscriptionFactory<TSubscription> subscriptionFactory)
			: this(subscriptionStore, subscriptionFactory, NullLogger<DefaultWebhookSubscriptionManager<TSubscription>>.Instance) {
		}

		public ILogger Logger { get; }

		private async Task<bool> SetStateAsync(string tenantId, string userId, string subscriptionId, bool active, CancellationToken cancellationToken) {
			try {
				var subscription = await subscriptionStore.FindByIdAsync(tenantId, subscriptionId, cancellationToken);
				if (subscription == null) {
					Logger.LogWarning("Could not find the subscription with ID {SubscriptionId} of Tenant {TenantId}: could not change state",
						subscriptionId, tenantId);

					throw new SubscriptionNotFoundException(subscriptionId);
				}

				if (subscription.IsActive == active) {
					var stateString = active ? "active" : "not active";
					Logger.LogInformation("The subscription {SubscriptionId} of Tenant {TenantId} is already {ActiveState}",
						subscriptionId, tenantId, stateString);

					return false;
				}

				await subscriptionStore.SetState(tenantId, subscription, active, cancellationToken);
				await subscriptionStore.UpdateAsync(tenantId, subscription);

				await OnSubscriptionStateChangesAsync(tenantId, userId, subscription, active, cancellationToken);

				return true;
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while trying to change the state of subscription {SubscriptionId} of tenant {TenantId}",
					subscriptionId, tenantId);
				throw;
			}
		}

		protected virtual Task OnSubscriptionStateChangesAsync(string tenantId, string userId, TSubscription subscription, bool active, CancellationToken cancellationToken) {
			return Task.CompletedTask;
		}

		protected virtual Task OnSubscriptionCreatedAsync(string tenantId, string userId, string id, TSubscription subscription, CancellationToken cancellationToken) {
			return Task.CompletedTask;
		}

		protected virtual Task OnSubscriptionDeletedAsync(string tenantId, string userId, TSubscription subscription, CancellationToken cancellationToken) {
			return Task.CompletedTask;
		}

		public virtual async Task<string> AddSubscriptionAsync(string tenantId, string userId, WebhookSubscriptionInfo subscriptionInfo, CancellationToken cancellationToken) {
			try {
				var subscription = subscriptionFactory.Create(subscriptionInfo);
				var result = await subscriptionStore.CreateAsync(tenantId, subscription, cancellationToken);

				Logger.LogInformation("New subscription with ID {SubscriptionId} for Tenant {TenantId}", result, tenantId);

				await OnSubscriptionCreatedAsync(tenantId, userId, result, subscription, cancellationToken);

				return result;
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while creating a subscription for Tenant {TenantId}", tenantId);
				throw;
			}
		}

		public virtual async Task<bool> RemoveSubscriptionAsync(string tenantId, string userId, string subscriptionId, CancellationToken cancellationToken) {
			try {
				var subscription = await subscriptionStore.FindByIdAsync(tenantId, subscriptionId, cancellationToken);

				if (subscription == null) {
					Logger.LogWarning("Trying to delete the subscription {SubscriptionId} of Tenant {TenantId}, but it was not found",
						subscriptionId, tenantId);

					throw new SubscriptionNotFoundException(subscriptionId);
				}

				var result = await subscriptionStore.DeleteAsync(tenantId, subscription);
				if (!result) {
					Logger.LogWarning("The subscription {SubscriptionId} of Tenant {TenantId} was not deleted from the store",
						subscriptionId, tenantId);
				} else {
					Logger.LogInformation("The subscription {SubscriptionId} of Tenant {TenantId} was deleted from the store");

					await OnSubscriptionDeletedAsync(tenantId, userId, subscription, cancellationToken);
				}

				return result;
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while delete subscription {SubscriptionId} of Tenant {TenantId}", subscriptionId, tenantId);
				throw;
			}
		}

		public virtual Task<bool> DisableSubscriptionAsync(string tenantId, string userId, string subscriptionId, CancellationToken cancellationToken)
			=> SetStateAsync(tenantId, userId, subscriptionId, false, cancellationToken);

		public virtual Task<bool> EnableSubscriptionAsync(string tenantId, string userId, string subscriptionId, CancellationToken cancellationToken)
			=> SetStateAsync(tenantId, userId, subscriptionId, true, cancellationToken);

		public virtual async Task<TSubscription> GetSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken) {
			try {
				return await subscriptionStore.FindByIdAsync(tenantId, subscriptionId, cancellationToken);
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while retrieving the webhook subscription {SubscriptionId} of tenant {TenantId}",
					subscriptionId, tenantId);
				throw;
			}
		}

		public virtual async Task<PaginatedResult<TSubscription>> GetSubscriptionsAsync(string tenantId, PageRequest page, CancellationToken cancellationToken) {
			try {
				return await subscriptionStore.GetPageAsync(tenantId, page, cancellationToken);
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while retrieving a page of subscriptions for tenant {TenantId}", tenantId);
				throw;
			}
		}
	}
}