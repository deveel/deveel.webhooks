// Copyright 2022-2024 Antonello Provenzano
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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Deveel.Webhooks {
	/// <summary>
	/// A base class that provides common functionality for a notifier
	/// to reach out receivers that subscribed for a given event.
	/// </summary>
	/// <typeparam name="TWebhook">
	/// The type of webhook notified to the subscribers.
	/// </typeparam>
	public abstract class WebhookNotifierBase<TWebhook> where TWebhook : class {
		private readonly IWebhookDeliveryResultLogger<TWebhook> deliveryResultLogger;
		private readonly IWebhookSender<TWebhook> sender;
		private readonly IWebhookFactory<TWebhook> webhookFactory;
		private readonly IDictionary<string, IWebhookFilterEvaluator<TWebhook>> filterEvaluators;

		internal WebhookNotifierBase(
			WebhookNotificationOptions<TWebhook> notificationOptions,
			IWebhookSender<TWebhook> sender,
			IWebhookFactory<TWebhook> webhookFactory,
			IEnumerable<IWebhookFilterEvaluator<TWebhook>>? filterEvaluators = null,
			IWebhookDeliveryResultLogger<TWebhook>? deliveryResultLogger = null,
			ILogger<TenantWebhookNotifier<TWebhook>>? logger = null) {
			NotificationOptions = notificationOptions;
			this.sender = sender;
			this.webhookFactory = webhookFactory;
			this.filterEvaluators = GetFilterEvaluators(filterEvaluators);
			this.deliveryResultLogger = deliveryResultLogger ?? NullWebhookDeliveryResultLogger<TWebhook>.Instance;
			Logger = logger ?? NullLogger<TenantWebhookNotifier<TWebhook>>.Instance;
		}

		/// <summary>
		/// Gets the logger used to log the activity of the notifier.
		/// </summary>
		protected ILogger Logger { get; }

		/// <summary>
		/// Gets the options used to configure the notification.
		/// </summary>
		protected WebhookNotificationOptions<TWebhook> NotificationOptions { get; }

		private static IDictionary<string, IWebhookFilterEvaluator<TWebhook>> GetFilterEvaluators(IEnumerable<IWebhookFilterEvaluator<TWebhook>>? filterEvaluators) {
			var evaluators = new Dictionary<string, IWebhookFilterEvaluator<TWebhook>>();

			if (filterEvaluators != null) {
				foreach (var filterEvaluator in filterEvaluators) {
					evaluators[filterEvaluator.Format] = filterEvaluator;
				}
			}

			return evaluators;
		}

		/// <summary>
		/// Creates a new webhook filter for the given subscription.
		/// </summary>
		/// <param name="subscription">
		/// The subscription to create the filter for.
		/// </param>
		/// <returns>
		/// Returns an instance of <see cref="WebhookSubscriptionFilter"/>
		/// </returns>
		protected virtual WebhookSubscriptionFilter? BuildSubscriptionFilter(IWebhookSubscription subscription) {
			return subscription.AsFilter();
		}

		/// <summary>
		/// Gets the filter evaluator for the given format.
		/// </summary>
		/// <param name="format">
		/// The format of the filter evaluator to get.
		/// </param>
		/// <returns>
		/// Returns an instance of <see cref="IWebhookFilterEvaluator{TWebhook}"/>
		/// that matches the given format, or <c>null</c> if no evaluator was
		/// found for the given format.
		/// </returns>
		protected virtual IWebhookFilterEvaluator<TWebhook>? GetFilterEvaluator(string format) {
			return !filterEvaluators.TryGetValue(format, out var filterEvaluator) ? null : filterEvaluator;
		}

		/// <summary>
		/// Matches the given webhook against the given filter.
		/// </summary>
		/// <param name="filter">
		/// The subscription filter to match the webhook against.
		/// </param>
		/// <param name="webhook">
		/// The webhook to match against the filter.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used to cancel the operation.
		/// </param>
		/// <remarks>
		/// The default implementation of this method invokes a filter evaluator
		/// if the following pre-conditions are met:
		/// <list type="bullet">
		/// <item>The filer is not empty or <c>null</c> (it returns <c>true</c>)</item>
		/// <item>The filter is not a wildcard (it returns <c>true</c>)</item>
		/// </list>
		/// </remarks>
		/// <returns>
		/// Returns <c>true</c> if the webhook matches the filter, <c>false</c> otherwise.
		/// </returns>
		/// <exception cref="NotSupportedException">
		/// Thrown when the filter format is not supported.
		/// </exception>
		protected virtual async Task<bool> MatchesAsync(WebhookSubscriptionFilter? filter, TWebhook webhook, CancellationToken cancellationToken) {
			if (filter == null || filter.IsEmpty) {
				Logger.TraceEmptyFilter();
				return true;
			}

			if (filter.IsWildcard) {
				Logger.TraceWildcardFilter();
				return true;
			}

			Logger.TraceSelectingEvaluator(filter.FilterFormat);

			var filterEvaluator = GetFilterEvaluator(filter.FilterFormat);

			if (filterEvaluator == null) {
				Logger.WarnEvaluatorNotFound(filter.FilterFormat);
				throw new NotSupportedException($"Filers of type '{filter.FilterFormat}' are not supported");
			}

			return await filterEvaluator.MatchesAsync(filter, webhook, cancellationToken);
		}

		/// <summary>
		/// A callback that is invoked after a webhook has been sent
		/// </summary>
		/// <param name="subscription">
		/// The subscription that was used to send the webhook.
		/// </param>
		/// <param name="webhook">
		/// The webhook that was sent.
		/// </param>
		/// <param name="result">
		/// The result of the delivery operation.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a task that completes when the operation is done.
		/// </returns>
		protected virtual Task OnDeliveryResultAsync(IWebhookSubscription subscription, TWebhook webhook, WebhookDeliveryResult<TWebhook> result, CancellationToken cancellationToken) {
			OnDeliveryResult(subscription, webhook, result);
			return Task.CompletedTask;
		}

		/// <summary>
		/// A callback that is invoked after a webhook has been sent
		/// </summary>
		/// <param name="subscription">
		/// The subscription that was used to send the webhook.
		/// </param>
		/// <param name="webhook">
		/// The webhook that was sent.
		/// </param>
		/// <param name="result">
		/// The result of the delivery operation.
		/// </param>
		protected virtual void OnDeliveryResult(IWebhookSubscription subscription, TWebhook webhook, WebhookDeliveryResult<TWebhook> result) {

		}

		/// <summary>
		/// Logs the given delivery result.
		/// </summary>
		/// <param name="eventInfo">
		/// The information about the event that triggered the notification.
		/// </param>
		/// <param name="subscription">
		/// The subscription that was used to send the webhook.
		/// </param>
		/// <param name="deliveryResult">
		/// The result of the delivery operation.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a task that completes when the operation is done.
		/// </returns>
		protected virtual async Task LogDeliveryResultAsync(EventNotification notification, IWebhookSubscription subscription, WebhookDeliveryResult<TWebhook> deliveryResult, CancellationToken cancellationToken) {
			try {
				if (deliveryResultLogger != null)
					await deliveryResultLogger.LogResultAsync(notification, subscription, deliveryResult, cancellationToken);
			} catch (Exception ex) {
				// If an error occurs here, we report it, but we don't throw it...
				Logger.LogUnknownEventDeliveryError(ex, subscription.SubscriptionId!, notification.EventType);
			}
		}

		private void TraceDeliveryResult(EventNotification notification, WebhookDeliveryResult<TWebhook> deliveryResult) {
			if (!deliveryResult.HasAttempted) {
				Logger.WarnDeliveryNotAttempted(deliveryResult.Destination.Url.GetLeftPart(UriPartial.Path), notification.EventType);
			} else if (deliveryResult.Successful) {
				Logger.TraceDeliveryDoneAfterAttempts(deliveryResult.Destination.Url.GetLeftPart(UriPartial.Path), notification.EventType, deliveryResult.Attempts.Count);
			} else {
				Logger.WarnDeliveryFailed(deliveryResult.Destination.Url.GetLeftPart(UriPartial.Path), notification.EventType, deliveryResult.Attempts.Count);
			}

			if (deliveryResult.HasAttempted) {
				foreach (var attempt in deliveryResult.Attempts) {
					if (attempt.Failed) {
						Logger.TraceDeliveryAttemptFailed(attempt.Number, attempt.StartedAt, attempt.CompletedAt, attempt.ResponseCode);
					} else {
						Logger.TraceDeliveryAttemptSuccess(attempt.Number, attempt.StartedAt, attempt.CompletedAt, attempt.ResponseCode);
					}
				}
			}
		}

		private async Task NotifySubscription(WebhookNotificationResult<TWebhook> result, EventNotification notification, IWebhookSubscription subscription, CancellationToken cancellationToken) {
			if (String.IsNullOrWhiteSpace(subscription.SubscriptionId))
				throw new WebhookException("The subscription identifier is missing");

			Logger.TraceEvaluatingSubscription(subscription.SubscriptionId, notification.EventType);

			var webhook = await CreateWebhook(subscription, notification, cancellationToken);

			if (webhook == null) {
				Logger.WarnWebhookNotCreated(subscription.SubscriptionId, notification.EventType);				
				return;
			}

			try {
				var filter = BuildSubscriptionFilter(subscription);

				if (await MatchesAsync(filter, webhook, cancellationToken)) {
					Logger.TraceSubscriptionMatched(subscription.SubscriptionId, notification.EventType);

					var deliveryResult = await SendAsync(subscription, webhook, cancellationToken);

					result.AddDelivery(subscription.SubscriptionId, deliveryResult);

					await LogDeliveryResultAsync(notification, subscription, deliveryResult, cancellationToken);

					TraceDeliveryResult(notification, deliveryResult);

					try {
						await OnDeliveryResultAsync(subscription, webhook, deliveryResult, cancellationToken);
					} catch (Exception ex) {
						Logger.LogUnknownEventDeliveryError(ex, subscription.SubscriptionId, notification.EventType);
					}
				} else {
					Logger.TraceSubscriptionNotMatched(subscription.SubscriptionId, notification.EventType);
				}
			} catch (Exception ex) {
				Logger.LogUnknownEventDeliveryError(ex, subscription.SubscriptionId, notification.EventType);

				await OnDeliveryErrorAsync(subscription, webhook, ex, cancellationToken);

				// result.AddDelivery(new WebhookDeliveryResult<TWebhook>(destination, webhook));
			}
		}

		/// <summary>
		/// Performs the notification of the given event to the subscriptions
		/// resolved that are listening for it.
		/// </summary>
		/// <param name="notification">
		/// The aggregate of the events to be notified to the subscribers.
		/// </param>
		/// <param name="subscriptions">
		/// The subscriptions that are listening for the event.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a task that completes when the operation is done.
		/// </returns>
		protected virtual async Task<WebhookNotificationResult<TWebhook>> NotifySubscriptionsAsync(EventNotification notification, IEnumerable<IWebhookSubscription> subscriptions, CancellationToken cancellationToken) {
			var result = new WebhookNotificationResult<TWebhook>(notification);

			// TODO: Make the parallel thread count configurable
			var options = new ParallelOptions { 
				MaxDegreeOfParallelism = NotificationOptions.ParallelThreadCount,
				CancellationToken = cancellationToken 
			};

			await Parallel.ForEachAsync(subscriptions, options, async (subscription, token) => {
				await NotifySubscription(result, notification, subscription, cancellationToken);
			});


			return result;

		}

		/// <summary>
		/// A callback that is invoked when a delivery error
		/// occurred during a notification
		/// </summary>
		/// <param name="subscription">
		/// The subscription that was being notified.
		/// </param>
		/// <param name="webhook">
		/// The webhook that was being delivered.
		/// </param>
		/// <param name="error">
		/// The error that occurred during the delivery.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a task that can be awaited.
		/// </returns>
		protected virtual Task OnDeliveryErrorAsync(IWebhookSubscription subscription, TWebhook webhook, Exception error, CancellationToken cancellationToken) {
			OnDeliveryError(subscription, webhook, error);
			return Task.CompletedTask;
		}

		/// <summary>
		/// A callback that is invoked when a delivery result
		/// </summary>
		/// <param name="subscription">
		/// The subscription that was being notified.
		/// </param>
		/// <param name="webhook">
		/// The webhook that was being delivered.
		/// </param>
		/// <param name="error">
		/// The error that occurred during the delivery.
		/// </param>
		protected virtual void OnDeliveryError(IWebhookSubscription subscription, TWebhook webhook, Exception error) {

		}

		/// <summary>
		/// A callback that is invoked when a delivery result
		/// </summary>
		/// <param name="subscription"></param>
		/// <param name="webhook"></param>
		/// <param name="cancellationToken"></param>
		/// <returns>
		/// Returns the result of a single delivery operation.
		/// </returns>
		protected virtual Task<WebhookDeliveryResult<TWebhook>> SendAsync(IWebhookSubscription subscription, TWebhook webhook, CancellationToken cancellationToken) {
			try {
				var destination = subscription.AsDestination();

				return sender.SendAsync(destination, webhook, cancellationToken);
			} catch (WebhookSenderException ex) {
				Logger.LogUnknownDeliveryError(ex, subscription.SubscriptionId!);
				throw;
			} catch (Exception ex) {
				Logger.LogUnknownDeliveryError(ex, subscription.SubscriptionId!);

				throw new WebhookException("An unknown error occurred when trying to send a webhook", ex);
			}
		}

		/// <summary>
		/// Creates a new webhook for the given subscription and event.
		/// </summary>
		/// <param name="subscription">
		/// The subscription that is being notified.
		/// </param>
		/// <param name="notification">
		/// The aggregate of the events to be notified to the subscribers.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a new webhook that can be delivered to the subscription,
		/// or <c>null</c> if it was not possible to constuct the data.
		/// </returns>
		/// <exception cref="WebhookException"></exception>
		protected virtual async Task<TWebhook?> CreateWebhook(IWebhookSubscription subscription, EventNotification notification, CancellationToken cancellationToken) {
			try {
				return await webhookFactory.CreateAsync(subscription, notification, cancellationToken);
			} catch (Exception ex) {
				Logger.LogWebhookCreationError(ex, subscription.SubscriptionId!, notification.EventType);
				throw new WebhookException("An error occurred while creating a new webhook", ex);
			}

		}
	}
}
