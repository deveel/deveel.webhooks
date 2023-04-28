﻿// Copyright 2022-2023 Deveel
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
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using System.Reflection;

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
		private readonly IEventTransformerPipeline? eventTransformer;
		private readonly IDictionary<string, IWebhookFilterEvaluator<TWebhook>> filterEvaluators;

		internal WebhookNotifierBase(
			IWebhookSender<TWebhook> sender,
			IWebhookFactory<TWebhook> webhookFactory,
			IEventTransformerPipeline? eventTransformer = null,
			IEnumerable<IWebhookFilterEvaluator<TWebhook>>? filterEvaluators = null,
			IWebhookDeliveryResultLogger<TWebhook>? deliveryResultLogger = null,
			ILogger<TenantWebhookNotifier<TWebhook>>? logger = null) {
			this.sender = sender;
			this.webhookFactory = webhookFactory;
			this.eventTransformer = eventTransformer;
			this.filterEvaluators = GetFilterEvaluators(filterEvaluators);
			this.deliveryResultLogger = deliveryResultLogger ?? NullWebhookDeliveryResultLogger<TWebhook>.Instance;
			Logger = logger ?? NullLogger<TenantWebhookNotifier<TWebhook>>.Instance;
		}

		/// <summary>
		/// Gets the logger used to log the activity of the notifier.
		/// </summary>
		protected ILogger Logger { get; }

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
		/// Transforms the data included in the event into an
		/// object that can be used to create a webhook.
		/// </summary>
		/// <param name="eventInfo">
		/// The information about the event that triggered the notification.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used to cancel the operation.
		/// </param>
		/// <remarks>
		/// This method is called before a webhook is created and sent: the
		/// generated data will be used to renew the instance of the
		/// <see cref="EventInfo"/>, that will then be constructed into the webhook.
		/// </remarks>
		/// <returns>
		/// Returns an object that will be used to renew the data of the event
		/// before passing it to the factory.
		/// </returns>
		/// <seealso cref="IWebhookFactory{TWebhook}"/>
		protected virtual async Task<object> GetWebhookDataAsync(EventInfo eventInfo, CancellationToken cancellationToken) {
			var data = eventInfo.Data;

			if (eventTransformer != null) {
				var result = await eventTransformer.TransformAsync(eventInfo, cancellationToken);

				data = result.Data;
			}

			return data;
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
				Logger.LogTrace("The filter request was null or empty: accepting by default");
				return true;
			}

			if (filter.IsWildcard) {
				Logger.LogTrace("The whole filter request was a wildcard");
				return true;
			}

			Logger.LogTrace("Selecting the filter evaluator for '{FilterFormat}' format", filter.FilterFormat);

			var filterEvaluator = GetFilterEvaluator(filter.FilterFormat);

			if (filterEvaluator == null) {
				Logger.LogError("Could not resolve any filter evaluator for the format '{FilterFormat}'", filter.FilterFormat);
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
		protected virtual async Task LogDeliveryResultAsync(IWebhookSubscription subscription, WebhookDeliveryResult<TWebhook> deliveryResult, CancellationToken cancellationToken) {
			try {
				if (deliveryResultLogger != null)
					await deliveryResultLogger.LogResultAsync(subscription, deliveryResult, cancellationToken);
			} catch (Exception ex) {
				// If an error occurs here, we report it, but we don't throw it...
				Logger.LogError(ex, "Error while logging a delivery result for tenant {TenantId}", subscription.TenantId);
			}
		}

		private void TraceDeliveryResult(WebhookDeliveryResult<TWebhook> deliveryResult) {
			if (!deliveryResult.HasAttempted) {
				Logger.LogTrace("The delivery was not attempted");
			} else if (deliveryResult.Successful) {
				Logger.LogTrace("The delivery was successful after {AttemptCount} attempts", deliveryResult.Attempts.Count());
			} else {
				Logger.LogTrace("The delivery failed after {AttemptCount} attempts", deliveryResult.Attempts.Count());
			}

			if (deliveryResult.HasAttempted) {
				foreach (var attempt in deliveryResult.Attempts) {
					if (attempt.Failed) {
						Logger.LogTrace("Attempt {AttemptNumber} Failed - [{StartDate} - {EndDate}] {StatusCode}: {ErrorMessage}",
							attempt.Number, attempt.StartedAt, attempt.CompletedAt, attempt.ResponseCode, attempt.ResponseMessage);
					} else {
						Logger.LogTrace("Attempt {AttemptNumber} Successful - [{StartDate} - {EndDate}] {StatusCode}",
							attempt.Number, attempt.StartedAt, attempt.CompletedAt, attempt.ResponseCode);
					}
				}
			}
		}

		private async Task NotifySubscription(WebhookNotificationResult<TWebhook> result, EventInfo eventInfo, IWebhookSubscription subscription, CancellationToken cancellationToken) {
			if (String.IsNullOrWhiteSpace(subscription.SubscriptionId))
				throw new WebhookException("The subscription identifier is missing");

			Logger.LogDebug("Evaluating subscription {SubscriptionId} to the event of type {EventType}",
				subscription.SubscriptionId, eventInfo.EventType);

			var webhook = await CreateWebhook(subscription, eventInfo, cancellationToken);

			if (webhook == null) {
				Logger.LogWarning("It was not possible to generate the webhook for the event {EventType} to be delivered to subscription {SubscriptionName} ({SubscriptionId})",
					eventInfo.EventType, subscription.Name, subscription.SubscriptionId);
				
				return;
			}

			try {
				var filter = BuildSubscriptionFilter(subscription);

				if (await MatchesAsync(filter, webhook, cancellationToken)) {
					Logger.LogTrace("Delivering webhook for event {EventType} to subscription {SubscriptionId}",
						eventInfo.EventType, subscription.SubscriptionId);

					var deliveryResult = await SendAsync(subscription, webhook, cancellationToken);

					result.AddDelivery(subscription.SubscriptionId, deliveryResult);

					await LogDeliveryResultAsync(subscription, deliveryResult, cancellationToken);

					TraceDeliveryResult(deliveryResult);

					try {
						await OnDeliveryResultAsync(subscription, webhook, deliveryResult, cancellationToken);
					} catch (Exception ex) {
						Logger.LogError(ex, "The event handling on the delivery thrown an error");
					}

				} else {
					Logger.LogTrace("The webhook for event {EventType} could not match the subscription {SubscriptionId}",
						eventInfo.EventType, subscription.SubscriptionId);
				}
			} catch (Exception ex) {
				Logger.LogError(ex, "Could not deliver a webhook for event {EventType} to subscription {SubscriptionId}",
					typeof(TWebhook), subscription.SubscriptionId);

				await OnDeliveryErrorAsync(subscription, webhook, ex, cancellationToken);

				// result.AddDelivery(new WebhookDeliveryResult<TWebhook>(destination, webhook));
			}
		}

		/// <summary>
		/// Performs the notification of the given event to the subscriptions
		/// resolved that are listening for it.
		/// </summary>
		/// <param name="eventInfo">
		/// The information about the event that is being notified.
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
		protected virtual async Task<WebhookNotificationResult<TWebhook>> NotifySubscriptionsAsync(EventInfo eventInfo, IEnumerable<IWebhookSubscription> subscriptions, CancellationToken cancellationToken) {
			var result = new WebhookNotificationResult<TWebhook>(eventInfo);

			// TODO: Make the parallel thread count configurable
			var options = new ParallelOptions { 
				MaxDegreeOfParallelism = Environment.ProcessorCount - 1, 
				CancellationToken = cancellationToken 
			};

			await Parallel.ForEachAsync(subscriptions, options, async (subscription, token) => {
				await NotifySubscription(result, eventInfo, subscription, cancellationToken);
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
				Logger.LogError(ex, "The webhook sender failed to send a webhook for event {EventType} to tenant {TenantId} because of an error",
					typeof(TWebhook), subscription.TenantId);
				throw;
			} catch (Exception ex) {
				Logger.LogError(ex, "An unknown error occurred when trying to send a webhook for event {EventType} to tenant {TenantId}",
										typeof(TWebhook), subscription.TenantId);

				throw new WebhookException("An unknown error occurred when trying to send a webhook", ex);
			}
		}

		/// <summary>
		/// Creates a new webhook for the given subscription and event.
		/// </summary>
		/// <param name="subscription">
		/// The subscription that is being notified.
		/// </param>
		/// <param name="eventInfo">
		/// The information about the event that is being notified.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a new webhook that can be delivered to the subscription,
		/// or <c>null</c> if it was not possible to constuct the data.
		/// </returns>
		/// <exception cref="WebhookException"></exception>
		protected virtual async Task<TWebhook?> CreateWebhook(IWebhookSubscription subscription, EventInfo eventInfo, CancellationToken cancellationToken) {
			object data;

			try {
				data = await GetWebhookDataAsync(eventInfo, cancellationToken);
			} catch (Exception ex) {
				Logger.LogError(ex, "Error setting the data for the event {EventType} to subscription {SubscriptionId}",
					eventInfo.EventType, subscription.SubscriptionId);

				throw new WebhookException("An error occurred while trying to create the webhook data", ex);
			}

			if (data == null) {
				Logger.LogWarning("It was not possible to generate data for the event of type {EventType}", eventInfo.EventType);
				return null;
			}

			var newEvent = eventInfo.WithData(data);

			try {
				return await webhookFactory.CreateAsync(subscription, newEvent, cancellationToken);
			} catch (Exception ex) {
				throw new WebhookException("An error occurred while creating a new webhook", ex);
			}

		}
	}
}