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

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Deveel.Webhooks {
	/// <summary>
	/// A manager of webhook subscriptions that provides a set of operations
	/// for the handling of entities in a store.
	/// </summary>
	/// <typeparam name="TSubscription">
	/// The type of the subscription handled by the manager.
	/// </typeparam>
	public class WebhookSubscriptionManager<TSubscription>
		where TSubscription : class, IWebhookSubscription {
		private readonly IHttpContextAccessor? httpContextAccessor;

		/// <summary>
		/// Creates a new instance of the manager wrapping a given store
		/// of webhook subscriptions entities
		/// </summary>
		/// <param name="subscriptionStore">
		/// The store of webhook subscriptions entities.
		/// </param>
		/// <param name="validators">
		/// An optional collection of validators to be used to validate
		/// webhook subscriptions before creating or updating them.
		/// </param>
		/// <param name="httpContextAccessor">
		/// An accessor to the current HTTP context.
		/// </param>
		/// <param name="logger">
		/// A logger to be used to log messages informing on the operations
		/// of the manager.
		/// </param>
		/// <exception cref="ArgumentNullException"></exception>
		public WebhookSubscriptionManager(
			IWebhookSubscriptionStore<TSubscription> subscriptionStore,
			IEnumerable<IWebhookSubscriptionValidator<TSubscription>>? validators = null,
			IHttpContextAccessor? httpContextAccessor = null,
			ILogger<WebhookSubscriptionManager<TSubscription>>? logger = null) {
			Store = subscriptionStore ?? throw new ArgumentNullException(nameof(subscriptionStore));
			Validators = validators;
			this.httpContextAccessor = httpContextAccessor;
			Logger = logger ?? NullLogger<WebhookSubscriptionManager<TSubscription>>.Instance;
		}

		/// <summary>
		/// Gets the logger used by the manager to log messages.
		/// </summary>
		protected ILogger Logger { get; }

		/// <summary>
		/// Gets the store of webhook subscriptions entities.
		/// </summary>
		protected IWebhookSubscriptionStore<TSubscription> Store { get; }

		/// <summary>
		/// Gets the collection of validators used to validate webhook
		/// </summary>
		protected IEnumerable<IWebhookSubscriptionValidator<TSubscription>>? Validators { get; }

		/// <summary>
		/// Gets the cancellation token to be used to cancel the current
		/// operation context.
		/// </summary>
		protected CancellationToken CancellationToken => httpContextAccessor?.HttpContext?.RequestAborted ?? default;

		/// <summary>
		/// Gets a value indicating whether the store supports paging.
		/// </summary>
		public bool SupportsPaging => Store is IWebhookSubscriptionPagedStore<TSubscription>;

		/// <summary>
		/// Gets a value indicating whether the store supports queries.
		/// </summary>
		public bool SupportsQueries => Store is IWebhookSubscriptionQueryableStore<TSubscription>;

		/// <summary>
		/// When the store supports paging, this gets the instance of the
		/// store to access pages of subscriptions.
		/// </summary>
		protected IWebhookSubscriptionPagedStore<TSubscription> PagedStore {
			get {
				if (!(Store is IWebhookSubscriptionPagedStore<TSubscription> pagedStore))
					throw new NotSupportedException("The store does not support paging");

				return pagedStore;
			}
		}

		/// <summary>
		/// When the store supports queries, this gets a queryable
		/// object used to query the subscriptions.
		/// </summary>
		public IQueryable<TSubscription> Subscriptions {
			get {
				if (!(Store is IWebhookSubscriptionQueryableStore<TSubscription> queryableStore))
					throw new NotSupportedException("The store does not support queries");

				return queryableStore.AsQueryable();
			}
		}

		/// <summary>
		/// Sets the new status of a subscription.
		/// </summary>
		/// <param name="subscription">
		/// The subscription to change the status.
		/// </param>
		/// <param name="status">
		/// The new status to set.
		/// </param>
		/// <returns>
		/// Returns <c>true</c> if the status was changed, or <c>false</c>
		/// </returns>
		/// <exception cref="WebhookException">
		/// Thrown if the status of the subscription could not be changed because of
		/// an unhanded exception.
		/// </exception>
		public async Task<bool> SetStatusAsync(TSubscription subscription, WebhookSubscriptionStatus status) {
			try {
				if (subscription.Status == status) {
					Logger.LogTrace("The subscription {SubscriptionId} is already {Status}", subscription.SubscriptionId, status);
					return false;
				}

				await Store.SetStatusAsync(subscription, status, CancellationToken);

				Logger.LogInformation("The status of subscription {SubscriptionId} was changed to {Status}", subscription.SubscriptionId, status);

				return true;
			} catch (WebhookException) {
				throw;
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while trying to change the state of subscription {SubscriptionId}", subscription.SubscriptionId);
				throw new WebhookException("Could not change the state of the subscription", ex);
			}
		}

		protected virtual Task OnSubscriptionStatusChangedAsync(TSubscription subscription, WebhookSubscriptionStatus newStatus) {
			return Task.CompletedTask;
		}

		/// <summary>
		/// Validates the webhook subscription.
		/// </summary>
		/// <param name="subscription">
		/// The subscription to validate.
		/// </param>
		/// <returns></returns>
		/// <exception cref="WebhookSubscriptionValidationException"></exception>
		protected virtual async Task ValidateSubscriptionAsync(TSubscription subscription) {
			var errors = new List<string>();

			if (Validators != null) {
				foreach (var validator in Validators) {
					var result = await validator.ValidateAsync(this, subscription, CancellationToken);
					if (!result.Successful) {
						if (result.Errors != null) {
							errors.AddRange(result.Errors);
						} else {
							errors.Add("The webhook subscription is invalid");
						}
					}
				}
			}

			if (errors.Count > 0)
				throw new WebhookSubscriptionValidationException(errors.ToArray());
		}

		public virtual async Task<string> CreateAsync(TSubscription subscription) {
			try {
				await ValidateSubscriptionAsync(subscription);

				var result = await Store.CreateAsync(subscription, CancellationToken);

				Logger.LogInformation("New subscription with ID {SubscriptionId}", result);

				await OnSubscriptionCreatedAsync(subscription);

				return result;
			} catch (WebhookException) {
				throw;
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while creating a subscription");
				throw new WebhookException("Could not create the subscription", ex);
			}
		}

		protected virtual Task OnSubscriptionCreatedAsync(TSubscription subscription) {
			return Task.CompletedTask;
		}

		public virtual async Task<bool> DeleteAsync(TSubscription subscription) {
			try {
				var id = await Store.GetIdAsync(subscription, CancellationToken);
				if (id == null) {
					Logger.LogWarning("Attempt to delete a subscription without identifier");
					return false;
				}

				var existing = await FindByIdAsync(id);

				if (existing == null) {
					Logger.LogWarning("The subscription {SubscriptionId} was not found in the store", id);
					return false;
				}

				var result = await Store.DeleteAsync(subscription, CancellationToken);

				if (!result) {
					Logger.LogWarning("The subscription {SubscriptionId} was not deleted from the store", id);
				} else {
					Logger.LogInformation("The subscription {SubscriptionId} was deleted from the store", id);

					await OnSubscriptionRemovedAsync(subscription);
				}

				return result;
			} catch (WebhookException) {
				throw;
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while delete subscription {SubscriptionId}", subscription.SubscriptionId);
				throw new WebhookException("Could not delete the subscription", ex);
			}
		}

		protected virtual Task OnSubscriptionRemovedAsync(TSubscription subscription) {
			return Task.CompletedTask;
		}

		public virtual async Task<bool> UpdateAsync(TSubscription subscription) {
			try {
				await ValidateSubscriptionAsync(subscription);

				var result = await Store.UpdateAsync(subscription, CancellationToken);

				if (!result) {
					Logger.LogWarning("The subscription {SubscriptionId} was not updated in the store", subscription.SubscriptionId);
				} else {
					Logger.LogInformation("The subscription {SubscriptionId} was updated in the store", subscription.SubscriptionId);
					await OnSubscriptionUpdatedAsync(subscription);
				}
				return result;
			} catch (WebhookException) {
				throw;
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while updating subscription {SubscriptionId}", subscription.SubscriptionId);
				throw new WebhookException("Could not update the subscription", ex);
			}
		}

		private Task OnSubscriptionUpdatedAsync(TSubscription subscription) {
			return Task.CompletedTask;
		}

		public virtual Task<bool> DisableAsync(TSubscription subscription)
			=> SetStatusAsync(subscription, WebhookSubscriptionStatus.Suspended);

		public virtual Task<bool> EnableAsync(TSubscription subscription) 
			=> SetStatusAsync(subscription, WebhookSubscriptionStatus.Active);


		public virtual async Task<TSubscription?> FindByIdAsync(string subscriptionId) {
			try {
				return await Store.FindByIdAsync(subscriptionId, CancellationToken);
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while retrieving the webhook subscription {SubscriptionId}", subscriptionId);
				throw new WebhookException("Could not retrieve the subscription", ex);
			}
		}

		public virtual async Task<PagedResult<TSubscription>> GetPageAsync(PagedQuery<TSubscription> query) {
			try {
				if (SupportsPaging)
					return await PagedStore.GetPageAsync(query, CancellationToken);

				if (SupportsQueries) {
					var querySet = Subscriptions.AsQueryable();
					if (query.Predicate != null)
						querySet = querySet.Where(query.Predicate);

					var totalCount = querySet.Count();
					var items = querySet
						.Skip(query.Offset)
						.Take(query.PageSize);

					return new PagedResult<TSubscription>(query, totalCount, items);
				}

				throw new NotSupportedException("Paged query is not supported by the store");
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while retrieving a page of subscriptions");
				throw new WebhookException("Could not retrieve the subscriptions", ex);
			}
		}

		public virtual async Task<int> CountAllAsync() {
			try {
				return await Store.CountAllAsync(CancellationToken);
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while trying to count all webhook subscriptions");
				throw new WebhookException("Could not count the subscriptions", ex);
			}
		}
	}
}
