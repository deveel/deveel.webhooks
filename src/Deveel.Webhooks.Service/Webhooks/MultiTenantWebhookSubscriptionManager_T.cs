// Copyright 2022-2023 Deveel
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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using static System.Formats.Asn1.AsnWriter;

namespace Deveel.Webhooks {
	public class MultiTenantWebhookSubscriptionManager<TSubscription>
		where TSubscription : class, IWebhookSubscription {
		private readonly IWebhookSubscriptionFactory<TSubscription> subscriptionFactory;

		protected MultiTenantWebhookSubscriptionManager(IWebhookSubscriptionStoreProvider<TSubscription> subscriptionStoreProvider,
			IWebhookSubscriptionFactory<TSubscription> subscriptionFactory,
			IEnumerable<IMultiTenantWebhookSubscriptionValidator<TSubscription>> validators, ILogger logger) {
			StoreProvider = subscriptionStoreProvider;
			Logger = logger;
			Validators = validators;

			this.subscriptionFactory = subscriptionFactory;
		}

		public MultiTenantWebhookSubscriptionManager(IWebhookSubscriptionStoreProvider<TSubscription> subscriptionStoreProvider,
			IWebhookSubscriptionFactory<TSubscription> subscriptionFactory,
			IEnumerable<IMultiTenantWebhookSubscriptionValidator<TSubscription>> validators,
			ILogger<MultiTenantWebhookSubscriptionManager<TSubscription>> logger)
			: this(subscriptionStoreProvider, subscriptionFactory, validators, (ILogger)logger) {
		}

		public MultiTenantWebhookSubscriptionManager(IWebhookSubscriptionStoreProvider<TSubscription> subscriptionStoreProvider,
			IWebhookSubscriptionFactory<TSubscription> subscriptionFactory,
			ILogger<MultiTenantWebhookSubscriptionManager<TSubscription>> logger)
			: this(subscriptionStoreProvider, subscriptionFactory, new IMultiTenantWebhookSubscriptionValidator<TSubscription>[0], (ILogger)logger) {
		}

		public MultiTenantWebhookSubscriptionManager(IWebhookSubscriptionStoreProvider<TSubscription> subscriptionStoreProvider,
			IWebhookSubscriptionFactory<TSubscription> subscriptionFactory,
			IEnumerable<IMultiTenantWebhookSubscriptionValidator<TSubscription>> validators)
			: this(subscriptionStoreProvider, subscriptionFactory, validators, NullLogger<WebhookSubscriptionManager<TSubscription>>.Instance) {
		}

		public MultiTenantWebhookSubscriptionManager(IWebhookSubscriptionStoreProvider<TSubscription> subscriptionStoreProvider,
			IWebhookSubscriptionFactory<TSubscription> subscriptionFactory)
			: this(subscriptionStoreProvider, subscriptionFactory, new IMultiTenantWebhookSubscriptionValidator<TSubscription>[0]) {
		}

		protected ILogger Logger { get; }

		protected IWebhookSubscriptionStoreProvider<TSubscription> StoreProvider { get; }

		protected IEnumerable<IMultiTenantWebhookSubscriptionValidator<TSubscription>> Validators { get; }

		private async Task<bool> SetStateAsync(string tenantId, string subscriptionId, WebhookSubscriptionStatus status, CancellationToken cancellationToken) {
			try {
				var subscription = await StoreProvider.FindByIdAsync(tenantId, subscriptionId, cancellationToken);
				if (subscription == null) {
					Logger.LogWarning("Could not find the subscription with ID {SubscriptionId}: could not change state", subscriptionId);

					throw new SubscriptionNotFoundException(subscriptionId);
				}

				return await SetStateAsync(tenantId, subscription, status, cancellationToken);
			} catch (WebhookException) {
				throw;
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while trying to change the state of subscription {SubscriptionId} of tenant {TenantId}", 
					subscriptionId, tenantId);
				throw new WebhookException("Could not change the state of the subscription", ex);
			}
		}


		public async Task<bool> SetStateAsync(string tenantId, TSubscription subscription, WebhookSubscriptionStatus status, CancellationToken cancellationToken) {
			try {
				if (subscription.Status == status) {
						Logger.LogTrace("The subscription {SubscriptionId} owned by tenant '{TenantId}' is already {Status}",
							subscription.SubscriptionId, tenantId, status);
					
					return false;
				}

				await StoreProvider.SetStateAsync(tenantId, subscription, status, cancellationToken);
				await StoreProvider.UpdateAsync(tenantId, subscription, cancellationToken);

				await OnSubscriptionStatusChangedAsync(tenantId, subscription, status, cancellationToken);

				Logger.LogInformation("The status of subscription {SubscriptionId} owned by tenant '{TenantId}' was changed to {Status}",
						subscription.SubscriptionId, tenantId, status);

				return true;
			} catch (WebhookException) {
				throw;
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while trying to change the state of subscription {SubscriptionId} of tenant {TenantId}", 
					subscription.SubscriptionId, tenantId);
				throw new WebhookException("Could not change the state of the subscription", ex);
			}
		}
		protected virtual Task OnSubscriptionStatusChangedAsync(string tenantId, TSubscription subscription, WebhookSubscriptionStatus status, CancellationToken cancellationToken) {
			return Task.CompletedTask;
		}

		protected virtual async Task ValidateSubscriptionAsync(string tenantId, TSubscription subscription, CancellationToken cancellationToken) {
			var errors = new List<string>();

			if (Validators != null) {
				foreach (var validator in Validators) {
					var result = await validator.ValidateAsync(this, tenantId, subscription, cancellationToken);
					if (!result.Successful)
						errors.AddRange(result.Errors);
				}
			}

			if (errors.Count > 0)
				throw new WebhookSubscriptionValidationException(errors.ToArray());
		}


		public virtual Task<string> AddSubscriptionAsync(string tenantId, WebhookSubscriptionInfo subscriptionInfo, CancellationToken cancellationToken) {
			try {
				var subscription = subscriptionFactory.Create(subscriptionInfo);

				return AddSubscriptionAsync(tenantId, subscription, cancellationToken);
			} catch (WebhookException) {
				throw;
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while creating a subscription");
				throw new WebhookException("Could not create the subscription", ex);
			}
		}

		public virtual async Task<string> AddSubscriptionAsync(string tenantId, TSubscription subscription, CancellationToken cancellationToken) {
			try {
				await ValidateSubscriptionAsync(tenantId, subscription, cancellationToken);

				var result = await StoreProvider.CreateAsync(tenantId, subscription, cancellationToken);

				Logger.LogInformation("New subscription with ID {SubscriptionId} was created for tenant {TenantId}", result, tenantId);

				await OnSubscriptionCreatedAsync(tenantId, subscription, cancellationToken);

				return result;
			} catch (WebhookException) {
				throw;
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while creating a subscription for tenant '{TenantId}'", tenantId);
				throw new WebhookException("Could not create the subscription", ex);
			}
		}

		protected virtual Task OnSubscriptionCreatedAsync(string tenantId, TSubscription subscription, CancellationToken cancellationToken) {
			return Task.CompletedTask;
		}

		public virtual async Task<bool> RemoveSubscriptionAsync(string tenantId, TSubscription subscription, CancellationToken cancellationToken) {
			try {
				var result = await StoreProvider.DeleteAsync(tenantId, subscription, cancellationToken);

				if (!result) {
					Logger.LogWarning("The subscription {SubscriptionId} of tenant '{TenantId}' was not deleted from the store", 
						subscription.SubscriptionId, tenantId);
				} else {
					Logger.LogInformation("The subscription {SubscriptionId} of tenant '{TenantId}' was deleted from the store", 
						subscription.SubscriptionId, tenantId);

					await OnSubscriptionRemovedAsync(tenantId, subscription, cancellationToken);
				}

				return result;
			} catch (WebhookException) {
				throw;
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while deleting subscription {SubscriptionId} of tenant {TenantId}", 
					subscription.SubscriptionId, tenantId);
				throw new WebhookException("Could not delete the subscription", ex);
			}
		}

		protected virtual Task OnSubscriptionRemovedAsync(string tenantId, TSubscription subscription, CancellationToken cancellationToken) {
			return Task.CompletedTask;
		}

		public virtual async Task<bool> RemoveSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken) {
			try {
				var subscription = await StoreProvider.FindByIdAsync(tenantId, subscriptionId, cancellationToken);

				if (subscription == null) {
					Logger.LogWarning("Trying to delete the subscription {SubscriptionId}, but it was not found", subscriptionId);

					throw new SubscriptionNotFoundException(subscriptionId);
				}

				return await RemoveSubscriptionAsync(tenantId, subscription, cancellationToken);
			} catch (WebhookException) {
				throw;
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while delete subscription {SubscriptionId}", subscriptionId);
				throw new WebhookException("Could not delete the subscription", ex);
			}
		}

		public virtual Task<bool> DisableSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken)
			=> SetStateAsync(tenantId, subscriptionId, WebhookSubscriptionStatus.Suspended, cancellationToken);

		public virtual Task<bool> DisableSubscriptionAsync(string tenantId, TSubscription subscription, CancellationToken cancellationToken)
			=> SetStateAsync(tenantId, subscription, WebhookSubscriptionStatus.Suspended, cancellationToken);

		public virtual Task<bool> EnableSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken)
			=> SetStateAsync(tenantId, subscriptionId, WebhookSubscriptionStatus.Active, cancellationToken);

		public virtual Task<bool> EnableSubscriptionAsync(string tenantId, TSubscription subscription, CancellationToken cancellationToken)
			=> SetStateAsync(tenantId, subscription, WebhookSubscriptionStatus.Active, cancellationToken);


		public virtual async Task<TSubscription> GetSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken) {
			try {
				return await StoreProvider.FindByIdAsync(tenantId, subscriptionId, cancellationToken);
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while retrieving the webhook subscription {SubscriptionId} of tenant {TenantId}", 
					subscriptionId, tenantId);
				throw new WebhookException("Could not retrieve the subscription", ex);
			}
		}

		public virtual async Task<PagedResult<TSubscription>> GetSubscriptionsAsync(string tenantId, PagedQuery<TSubscription> query, CancellationToken cancellationToken) {
			try {
				var store = StoreProvider.GetTenantStore(tenantId);
				if (store is IWebhookSubscriptionPagedStore<TSubscription> paged)
					return await paged.GetPageAsync(query, cancellationToken);

				if (store is IWebhookSubscriptionQueryableStore<TSubscription> queryable) {
					var totalCount = queryable.AsQueryable().Count(query.Predicate);
					var items = queryable.AsQueryable()
						.Skip(query.Offset)
						.Take(query.PageSize)
						.Cast<TSubscription>();

					return new PagedResult<TSubscription>(query, totalCount, items);
				}

				throw new NotSupportedException("Paged query is not supported by the store");
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while retrieving a page of subscriptions for tenant {TenantId}", tenantId);
				throw new WebhookException("Could not retrieve the subscriptions", ex);
			}
		}

		public virtual async Task<int> CountAllAsync(string tenantId, CancellationToken cancellationToken) {
			try {
				return await StoreProvider.CountAllAsync(tenantId, cancellationToken);
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while trying to count all webhook subscriptions");
				throw new WebhookException("Could not count the subscriptions", ex);
			}
		}
	}
}
