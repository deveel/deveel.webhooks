﻿// Copyright 2022 Deveel
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

namespace Deveel.Webhooks {
	/// <summary>
	/// The default implementation of the webhook notifier
	/// </summary>
	public class WebhookNotifier : IWebhookNotifier {
		private readonly IWebhookServiceConfiguration configuration;
		private readonly IWebhookSubscriptionResolver subscriptionResolver;
		private readonly IWebhookDeliveryResultLogger deliveryResultLogger;
		private readonly IWebhookSender sender;

		#region .ctor

		public WebhookNotifier(
			IWebhookServiceConfiguration configuration,
			IWebhookSender sender,
			IWebhookSubscriptionResolver subscriptionResolver,
			IWebhookDeliveryResultLogger deliveryResultLogger,
			ILogger<WebhookNotifier> logger) {
			this.sender = sender;
			this.subscriptionResolver = subscriptionResolver;
			this.configuration = configuration;
			this.deliveryResultLogger = deliveryResultLogger;
			Logger = logger;
		}

		public WebhookNotifier(
			IWebhookServiceConfiguration configuration,
			IWebhookSender sender,
			IWebhookSubscriptionResolver subscriptionResolver,
			ILogger<WebhookNotifier> logger)
			: this(configuration, sender, subscriptionResolver, null, logger) {
		}

		public WebhookNotifier(
			IWebhookServiceConfiguration configuration,
			IWebhookSender sender,
			IWebhookSubscriptionResolver subscriptionResolver,
			IWebhookDeliveryResultLogger deliveryResultLogger)
			: this(configuration, sender, subscriptionResolver, deliveryResultLogger, NullLogger<WebhookNotifier>.Instance) {
		}

		public WebhookNotifier(
			IWebhookServiceConfiguration configuration,
			IWebhookSender sender,
			IWebhookSubscriptionResolver subscriptionResolver)
			: this(configuration, sender, subscriptionResolver, (IWebhookDeliveryResultLogger) null) {
		}

		#endregion

		protected ILogger Logger { get; }

		protected virtual WebhookFilterRequest BuildFilterRequest(IWebhookSubscription subscription) {
			return WebookFilterRequestFactory.CreateRequest(subscription);
		}

		protected virtual async Task<object> GetWebhookDataAsync(EventInfo eventInfo, CancellationToken cancellationToken) {
			Logger.LogDebug("GetWebhookDataAsync: getting data for an event");

			var factory = configuration.DataFactories.Find(eventInfo);
			if (factory != null) {
				Logger.LogDebug("GetWebhookDataAsync: using a factory for the event of type {EventType} to generate the webhook data",
					eventInfo.EventType);

				return await factory.CreateDataAsync(eventInfo, cancellationToken);
			}

			Logger.LogDebug("GetWebhookDataAsync: using the data of the event");

			return eventInfo.Data;
		}

		protected virtual async Task<bool> MatchesAsync(WebhookFilterRequest filterRequest, IWebhook webhook, CancellationToken cancellationToken) {
			if (filterRequest == null || filterRequest.IsEmpty) {
				Logger.LogTrace("The filter request was null or empty: accepting by default");
				return true;
			}

			if (filterRequest.IsWildcard) {
				Logger.LogTrace("The whole filter request was a wildcard");
				return true;
			}

			Logger.LogTrace("Selecting the filter evaluator for '{FilterFormat}' format", filterRequest.FilterFormat);

			var filterEvaluator = configuration.FilterEvaluators[filterRequest.FilterFormat];

			if (filterEvaluator == null) {
				Logger.LogError("Could not resolve any filter evaluator for the format '{FilterFormat}'", filterRequest.FilterFormat);
				throw new NotSupportedException($"Filers of type '{filterRequest.FilterFormat}' are not supported");
			}

			return await filterEvaluator.MatchesAsync(filterRequest, webhook, cancellationToken);
		}

		protected virtual Task OnWebhookDeliveryResultAsync(string tenantId, IWebhookSubscription subscription, IWebhook webhook, WebhookDeliveryResult result, CancellationToken cancellationToken) {
			OnWebhookDeliveryResult(tenantId, subscription, webhook, result);
			return Task.CompletedTask;
		}

		protected virtual void OnWebhookDeliveryResult(string tenantId, IWebhookSubscription subscription, IWebhook webhook, WebhookDeliveryResult result) {

		}

		protected virtual async Task LogDeliveryResult(string tenantId, IWebhookDeliveryResult deliveryResult, CancellationToken cancellationToken) {
			try {
				if (deliveryResultLogger != null)
					await deliveryResultLogger.LogResultAsync(tenantId, deliveryResult, cancellationToken);
			} catch (Exception ex) {
				// If an error occurs here, we report it, but we don't throw it...
				Logger.LogError(ex, "Error while logging a delivery result for tenant {TenantId}", tenantId);
			}
		}

		private void TraceDeliveryResult(WebhookDeliveryResult deliveryResult) {
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
							attempt.Number, attempt.StartedAt, attempt.EndedAt, attempt.ResponseStatusCode, attempt.ResponseMessage);
					} else {
						Logger.LogTrace("Attempt {AttemptNumber} Successful - [{StartDate} - {EndDate}] {StatusCode}",
							attempt.Number, attempt.StartedAt, attempt.EndedAt, attempt.ResponseStatusCode);
					}
				}
			}
		}

		protected virtual async Task<IList<IWebhookSubscription>> ResolveSubscriptionsAsync(string tenantId, EventInfo eventInfo, CancellationToken cancellationToken) {
			return await subscriptionResolver.ResolveSubscriptionsAsync(tenantId, eventInfo.EventType, true, cancellationToken);
		}

		public virtual async Task<WebhookNotificationResult> NotifyAsync(string tenantId, EventInfo eventInfo, CancellationToken cancellationToken) {
			var result = new WebhookNotificationResult();

			try {
				var subscriptions = await ResolveSubscriptionsAsync(tenantId, eventInfo, cancellationToken);

				if (subscriptions == null || subscriptions.Count == 0) {
					Logger.LogTrace("No active subscription to event '{EventType}' found for Tenant '{TenantId}': skipping notification", eventInfo.EventType, tenantId);
					return result;
				}

				foreach (var subscription in subscriptions) {
					Logger.LogDebug("Evaluating subscription {SubscriptionId} to the event of type {EventType} of tenant {TenantId}",
						subscription.SubscriptionId, eventInfo.EventType, tenantId);

					var webhook = await CreateWebhook(subscription, eventInfo, cancellationToken);

					if (webhook == null) {
						Logger.LogWarning("It was not possible to generate the webhook for the event {EventType} to be delivered to subscription {SubscriptionName} ({SubscriptionId})",
							eventInfo.EventType, subscription.Name, subscription.SubscriptionId);
						continue;
					}

					try {
						var filterRequest = BuildFilterRequest(subscription);

						if (await MatchesAsync(filterRequest, webhook, cancellationToken)) {
							Logger.LogTrace("Delivering webhook for event {EventType} to subscription {SubscriptionId} of Tenant {TenantId}",
								eventInfo.EventType, subscription.SubscriptionId, tenantId);

							var deliveryResult = await SendAsync(tenantId, webhook, cancellationToken);

							result.AddDelivery(deliveryResult);

							await LogDeliveryResult(tenantId, deliveryResult, cancellationToken);

							TraceDeliveryResult(deliveryResult);

							try {
								await OnWebhookDeliveryResultAsync(tenantId, subscription, webhook, deliveryResult, cancellationToken);
							} catch (Exception ex) {
								Logger.LogError(ex, "The event handling on the delivery thrown an error");
							}

						} else {
							Logger.LogTrace("The webhook for event {EventType} could not match the subscription {SubscriptionId} of Tenant {TenantId}",
								eventInfo.EventType, subscription.SubscriptionId, tenantId);
						}
					} catch (Exception ex) {
						Logger.LogError(ex, "Could not deliver a webhook for event {EventType} to subscription {SubscriptionId} of Tenant {TenantId}",
							webhook.EventType, subscription.SubscriptionId, tenantId);

						await OnWebhookDeliveryErrorAsync(tenantId, subscription, webhook, ex, cancellationToken);

						result.AddDelivery(WebhookDeliveryResult.Fail(webhook));
					}
				}

				return result;
			} catch (Exception ex) {
				Logger.LogError(ex, "An unknown error when trying to notify the event {EventType} to tenant {TenantId}", eventInfo.EventType, tenantId);
				throw;
			}
		}

		protected virtual Task OnWebhookDeliveryErrorAsync(string tenantId, IWebhookSubscription subscription, IWebhook webhook, Exception error, CancellationToken cancellationToken) {
			return Task.CompletedTask;
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

		protected virtual async Task<IWebhook> CreateWebhook(IWebhookSubscription subscription, EventInfo eventInfo, CancellationToken cancellationToken) {
			object data;

			try {
				data = await GetWebhookDataAsync(eventInfo, cancellationToken);
			} catch (Exception ex) {
				Logger.LogError(ex, "Error setting the data for the event {EventType} to subscription {SubscriptionId}",
					eventInfo.EventType, subscription.SubscriptionId);
				throw;
			}

			if (data == null) {
				Logger.LogWarning("It was not possible to generate data for the event of type {EventType}", eventInfo.EventType);
				return null;
			}

			return new Webhook {
				SubscriptionId = subscription.SubscriptionId,
				Name = subscription.Name,
				EventType = eventInfo.EventType,
				DestinationUrl = subscription.DestinationUrl,
				Headers = subscription.Headers == null
					? null
					: new Dictionary<string, string>(subscription.Headers),
				Data = data,
				Secret = subscription.Secret,
				Id = eventInfo.Id,
				TimeStamp = eventInfo.TimeStamp
			};
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

			public string Format { get; set; }
		}

		#endregion
	}
}
