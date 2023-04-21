using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using static System.Formats.Asn1.AsnWriter;

namespace Deveel.Webhooks {
	public class WebhookSubscriptionManager<TSubscription>
		where TSubscription : class, IWebhookSubscription {

		private readonly IWebhookSubscriptionFactory<TSubscription> subscriptionFactory;

		protected WebhookSubscriptionManager(IWebhookSubscriptionStore<TSubscription> subscriptionStore, 
			IWebhookSubscriptionFactory<TSubscription> subscriptionFactory, 
			IEnumerable<IWebhookSubscriptionValidator<TSubscription>> validators, ILogger logger) {
			Store = subscriptionStore;
			Logger = logger;
			Validators = validators;

			this.subscriptionFactory = subscriptionFactory;
		}

		public WebhookSubscriptionManager(IWebhookSubscriptionStore<TSubscription> subscriptionStore,
			IWebhookSubscriptionFactory<TSubscription> subscriptionFactory,
			 IEnumerable<IWebhookSubscriptionValidator<TSubscription>> validators,
			ILogger<WebhookSubscriptionManager<TSubscription>> logger)
			: this(subscriptionStore, subscriptionFactory, validators, (ILogger)logger) {
		}

		public WebhookSubscriptionManager(IWebhookSubscriptionStore<TSubscription> subscriptionStore,
			IWebhookSubscriptionFactory<TSubscription> subscriptionFactory,
			ILogger<WebhookSubscriptionManager<TSubscription>> logger)
			: this(subscriptionStore, subscriptionFactory, new IWebhookSubscriptionValidator<TSubscription>[0], (ILogger)logger) {
		}

		public WebhookSubscriptionManager(IWebhookSubscriptionStore<TSubscription> subscriptionStore,
			IWebhookSubscriptionFactory<TSubscription> subscriptionFactory, 
			IEnumerable<IWebhookSubscriptionValidator<TSubscription>> validators)
			: this(subscriptionStore, subscriptionFactory, validators, NullLogger<WebhookSubscriptionManager<TSubscription>>.Instance) {
		}

		public WebhookSubscriptionManager(IWebhookSubscriptionStore<TSubscription> subscriptionStore,
			IWebhookSubscriptionFactory<TSubscription> subscriptionFactory)
			: this(subscriptionStore, subscriptionFactory, new IWebhookSubscriptionValidator<TSubscription>[0]) {
		}

		protected ILogger Logger { get; }

		protected IWebhookSubscriptionStore<TSubscription> Store { get; }

		protected IEnumerable<IWebhookSubscriptionValidator<TSubscription>> Validators { get; }

		private async Task<bool> SetStateAsync(string subscriptionId, WebhookSubscriptionStatus status, CancellationToken cancellationToken) {
			try {
				var subscription = await Store.FindByIdAsync(subscriptionId, cancellationToken);
				if (subscription == null) {
					Logger.LogWarning("Could not find the subscription with ID {SubscriptionId}: could not change state", subscriptionId);

					throw new SubscriptionNotFoundException(subscriptionId);
				}

				return await SetStateAsync(subscription, status, cancellationToken);
			} catch(WebhookException) {
				throw;
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while trying to change the state of subscription {SubscriptionId}", subscriptionId);
				throw new WebhookException("Could not change the state of the subscription", ex);
			}
		}

		public async Task<bool> SetStateAsync(TSubscription subscription, WebhookSubscriptionStatus status, CancellationToken cancellationToken) {
			try {
				if (subscription.Status == status) {
					Logger.LogTrace("The subscription {SubscriptionId} is already {Status}", subscription.SubscriptionId, status);
					return false;
				}

				await Store.SetStateAsync(subscription, status, cancellationToken);
				await Store.UpdateAsync(subscription, cancellationToken);

				await OnSubscriptionStatusChangedAsync(subscription, status, cancellationToken);

				Logger.LogInformation("The status of subscription {SubscriptionId} was changed to {Status}", subscription.SubscriptionId, status);

				return true;
			} catch (WebhookException) {
				throw;
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while trying to change the state of subscription {SubscriptionId}", subscription.SubscriptionId);
				throw new WebhookException("Could not change the state of the subscription", ex);
			}
		}
		protected virtual Task OnSubscriptionStatusChangedAsync(TSubscription subscription, WebhookSubscriptionStatus newStatus, CancellationToken cancellationToken) {
			return Task.CompletedTask;
		}

		protected virtual async Task ValidateSubscriptionAsync(TSubscription subscription, CancellationToken cancellationToken) {
			var errors = new List<string>();

			if (Validators != null) {
				foreach (var validator in Validators) {
					var result = await validator.ValidateAsync(this, subscription, cancellationToken);
					if (!result.Successful)
						errors.AddRange(result.Errors);
				}
			}

			if (errors.Count > 0)
				throw new WebhookSubscriptionValidationException(errors.ToArray());
		}

		public virtual Task<string> AddSubscriptionAsync(WebhookSubscriptionInfo subscriptionInfo, CancellationToken cancellationToken) {
			try {
				var subscription = subscriptionFactory.Create(subscriptionInfo);

				return AddSubscriptionAsync(subscription, cancellationToken);
			} catch (WebhookException) {
				throw;
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while creating a subscription");
				throw new WebhookException("Could not create the subscription", ex);
			}
		}

		public virtual async Task<string> AddSubscriptionAsync(TSubscription subscription, CancellationToken cancellationToken) {
			try {
				await ValidateSubscriptionAsync(subscription, cancellationToken);

				var result = await Store.CreateAsync(subscription, cancellationToken);

				Logger.LogInformation("New subscription with ID {SubscriptionId}", result);

				await OnSubscriptionCreatedAsync(subscription, cancellationToken);

				return result;
			} catch (WebhookException) {
				throw;
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while creating a subscription");
				throw new WebhookException("Could not create the subscription", ex);
			}
		}

		protected virtual Task OnSubscriptionCreatedAsync(TSubscription subscription, CancellationToken cancellationToken) {
			return Task.CompletedTask;
		}

		public virtual async Task<bool> RemoveSubscriptionAsync(TSubscription subscription, CancellationToken cancellationToken) {
			try {
				var result = await Store.DeleteAsync(subscription, cancellationToken);

				if (!result) {
					Logger.LogWarning("The subscription {SubscriptionId} was not deleted from the store", subscription.SubscriptionId);
				} else {
					Logger.LogInformation("The subscription {SubscriptionId} was deleted from the store", subscription.SubscriptionId);

					await OnSubscriptionRemovedAsync(subscription, cancellationToken);
				}

				return result;
			} catch (WebhookException) {
				throw;
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while delete subscription {SubscriptionId}", subscription.SubscriptionId);
				throw new WebhookException("Could not delete the subscription", ex);
			}
		}

		protected virtual Task OnSubscriptionRemovedAsync(TSubscription subscription, CancellationToken cancellationToken) {
			return Task.CompletedTask;
		}

		public virtual async Task<bool> RemoveSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken) {
			try {
				var subscription = await Store.FindByIdAsync(subscriptionId, cancellationToken);

				if (subscription == null) {
					Logger.LogWarning("Trying to delete the subscription {SubscriptionId}, but it was not found", subscriptionId);

					throw new SubscriptionNotFoundException(subscriptionId);
				}

				return await RemoveSubscriptionAsync(subscription, cancellationToken);
			} catch(WebhookException) {
				throw;
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while delete subscription {SubscriptionId}", subscriptionId);
				throw new WebhookException("Could not delete the subscription", ex);
			}
		}

		public virtual Task<bool> DisableSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken)
			=> SetStateAsync(subscriptionId, WebhookSubscriptionStatus.Suspended, cancellationToken);

		public virtual Task<bool> DisableSubscriptionAsync(TSubscription subscription, CancellationToken cancellationToken)
			=> SetStateAsync(subscription, WebhookSubscriptionStatus.Suspended, cancellationToken);

		public virtual Task<bool> EnableSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken)
			=> SetStateAsync(subscriptionId, WebhookSubscriptionStatus.Active, cancellationToken);

		public virtual Task<bool> EnableSubscriptionAsync(TSubscription subscription, CancellationToken cancellationToken)
			=> SetStateAsync(subscription, WebhookSubscriptionStatus.Active, cancellationToken);


		public virtual async Task<TSubscription> GetSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken) {
			try {
				return await Store.FindByIdAsync(subscriptionId, cancellationToken);
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while retrieving the webhook subscription {SubscriptionId}", subscriptionId);
				throw new WebhookException("Could not retrieve the subscription", ex);
			}
		}

		public virtual async Task<PagedResult<TSubscription>> GetSubscriptionsAsync(PagedQuery<TSubscription> query, CancellationToken cancellationToken) {
			try {
				if (Store is IWebhookSubscriptionPagedStore<TSubscription> paged)
					return await paged.GetPageAsync(query, cancellationToken);

				if (Store is IWebhookSubscriptionQueryableStore<TSubscription> queryable) {
					var totalCount = queryable.AsQueryable().Count(query.Predicate);
					var items = queryable.AsQueryable()
						.Skip(query.Offset)
						.Take(query.PageSize)
						.Cast<TSubscription>();

					return new PagedResult<TSubscription>(query, totalCount, items);
				}

				throw new NotSupportedException("Paged query is not supported by the store");
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while retrieving a page of subscriptions");
				throw new WebhookException("Could not retrieve the subscriptions", ex);
			}
		}

		public virtual async Task<int> CountAllAsync(CancellationToken cancellationToken) {
			try {
				return await Store.CountAllAsync(cancellationToken);
			} catch (Exception ex) {
				Logger.LogError(ex, "Error while trying to count all webhook subscriptions");
				throw new WebhookException("Could not count the subscriptions", ex);
			}
		}
	}
}
