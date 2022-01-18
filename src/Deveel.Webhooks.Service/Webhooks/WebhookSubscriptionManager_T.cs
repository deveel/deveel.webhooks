// Copyright 2022 Deveel
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// Copyright 2022 Deveel
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Webhooks.Storage;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Deveel.Webhooks {
	public class WebhookSubscriptionManager<TSubscription> : IWebhookSubscriptionManager<TSubscription>
		where TSubscription : class, IWebhookSubscription {
		private readonly IWebhookSubscriptionFactory<TSubscription> subscriptionFactory;
		private readonly IWebhookSubscriptionStoreProvider<TSubscription> subscriptionStore;

		protected WebhookSubscriptionManager(IWebhookSubscriptionStoreProvider<TSubscription> subscriptionStore,
			IWebhookSubscriptionFactory<TSubscription> subscriptionFactory,
			ILogger logger) {
			this.subscriptionStore = subscriptionStore;
			this.subscriptionFactory = subscriptionFactory;
			Logger = logger;
		}

		public WebhookSubscriptionManager(IWebhookSubscriptionStoreProvider<TSubscription> subscriptionStore,
			IWebhookSubscriptionFactory<TSubscription> subscriptionFactory,
			ILogger<WebhookSubscriptionManager<TSubscription>> logger)
			: this(subscriptionStore, subscriptionFactory, (ILogger)logger) {
		}

		public WebhookSubscriptionManager(IWebhookSubscriptionStoreProvider<TSubscription> subscriptionStore,
			IWebhookSubscriptionFactory<TSubscription> subscriptionFactory)
			: this(subscriptionStore, subscriptionFactory, NullLogger<WebhookSubscriptionManager<TSubscription>>.Instance) {
		}

		public ILogger Logger { get; }

		private async Task<bool> SetStateAsync(string tenantId, string userId, string subscriptionId, WebhookSubscriptionStatus status, CancellationToken cancellationToken) {
			try {
				var subscription = await subscriptionStore.GetByIdAsync(tenantId, subscriptionId, cancellationToken);
				if (subscription == null) {
					Logger.LogWarning("Could not find the subscription with ID {SubscriptionId} of Tenant {TenantId}: could not change state",
						subscriptionId, tenantId);

					throw new SubscriptionNotFoundException(subscriptionId);
				}

				if (subscription.Status == status) {
					Logger.LogTrace("The subscription {SubscriptionId} of Tenant {TenantId} is already {Status}",
						subscriptionId, tenantId, status);

					return false;
				}

				var stateInfo = new WebhookSubscriptionStateInfo(status, userId);

				await subscriptionStore.SetStateAsync(tenantId, subscription, stateInfo, cancellationToken);
				await subscriptionStore.UpdateAsync(tenantId, subscription, cancellationToken);

				await OnSubscriptionStateChangesAsync(tenantId, userId, subscription, status, cancellationToken);

				return true;
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while trying to change the state of subscription {SubscriptionId} of tenant {TenantId}",
					subscriptionId, tenantId);
				throw;
			}
		}

		protected virtual Task OnSubscriptionStateChangesAsync(string tenantId, string userId, TSubscription subscription, WebhookSubscriptionStatus status, CancellationToken cancellationToken) {
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
				var subscription = await subscriptionStore.GetByIdAsync(tenantId, subscriptionId, cancellationToken);

				if (subscription == null) {
					Logger.LogWarning("Trying to delete the subscription {SubscriptionId} of Tenant {TenantId}, but it was not found",
						subscriptionId, tenantId);

					throw new SubscriptionNotFoundException(subscriptionId);
				}

				var result = await subscriptionStore.DeleteAsync(tenantId, subscription, cancellationToken);

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
			=> SetStateAsync(tenantId, userId, subscriptionId, WebhookSubscriptionStatus.Suspended, cancellationToken);

		public virtual Task<bool> EnableSubscriptionAsync(string tenantId, string userId, string subscriptionId, CancellationToken cancellationToken)
			=> SetStateAsync(tenantId, userId, subscriptionId, WebhookSubscriptionStatus.Active, cancellationToken);

		public virtual async Task<TSubscription> GetSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken) {
			try {
				return await subscriptionStore.GetByIdAsync(tenantId, subscriptionId, cancellationToken);
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while retrieving the webhook subscription {SubscriptionId} of tenant {TenantId}",
					subscriptionId, tenantId);
				throw;
			}
		}

		public virtual async Task<WebhookSubscriptionPage<TSubscription>> GetSubscriptionsAsync(string tenantId, WebhookSubscriptionQuery<TSubscription> query, CancellationToken cancellationToken) {
			try {
				var store = subscriptionStore.GetTenantStore(tenantId);
				if (store is IWebhookSubscriptionPaginatedStore<TSubscription> paginated)
					return await paginated.GetPageAsync(query, cancellationToken);
				if (store is IWebhookSubscriptionQueryableStore<TSubscription> queryable) {
					var totalCount = queryable.AsQueryable().Count(query.Predicate);
					var items = queryable.AsQueryable()
						.Skip(query.Offset)
						.Take(query.PageSize)
						.Cast<TSubscription>();

					return new WebhookSubscriptionPage<TSubscription>(query, totalCount, items);
				}

				throw new NotSupportedException("Paged query is not supported by the store");
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while retrieving a page of subscriptions for tenant {TenantId}", tenantId);
				throw;
			}
		}
	}
}
