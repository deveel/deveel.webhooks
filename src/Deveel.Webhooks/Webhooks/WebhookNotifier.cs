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
		private readonly IWebhookSubscriptionResolver subscriptionResolver;
		private readonly IWebhookFilterSelector filterSelector;
		private readonly IWebhookDataFactorySelector dataStrategy;
		private readonly IWebhookSender sender;

		#region .ctor

		public WebhookNotifier(IWebhookSender sender,
			IWebhookSubscriptionResolver subscriptionResolver,
			IWebhookFilterSelector filterSelector,
			IWebhookDataFactorySelector dataStrategy,
			ILogger<WebhookNotifier> logger) {
			this.sender = sender;
			this.subscriptionResolver = subscriptionResolver;
			this.filterSelector = filterSelector;
			this.dataStrategy = dataStrategy;
			Logger = logger;
		}

		public WebhookNotifier(IWebhookSender sender,
			IWebhookSubscriptionResolver subscriptionResolver,
			IWebhookFilterSelector filterSelector,
			IWebhookDataFactorySelector dataStrategy)
			: this(sender, subscriptionResolver, filterSelector, dataStrategy, NullLogger<WebhookNotifier>.Instance) {
		}

		public WebhookNotifier(IWebhookSender sender,
			IWebhookSubscriptionResolver subscriptionResolver,
			IWebhookFilterSelector filterSelector,
			ILogger<WebhookNotifier> logger)
			: this(sender, subscriptionResolver, filterSelector, null, logger) {
		}

		public WebhookNotifier(IWebhookSender sender,
			IWebhookSubscriptionResolver subscriptionResolver,
			IWebhookDataFactorySelector dataStrategy,
			ILogger<WebhookNotifier> logger)
			: this(sender, subscriptionResolver, null, dataStrategy, logger) {
		}

		public WebhookNotifier(IWebhookSender sender,
			IWebhookSubscriptionResolver subscriptionResolver,
			IWebhookDataFactorySelector dataStrategy)
			: this(sender, subscriptionResolver, null, dataStrategy, NullLogger<WebhookNotifier>.Instance) {
		}

		public WebhookNotifier(IWebhookSender sender,
			IWebhookSubscriptionResolver subscriptionResolver)
			: this(sender, subscriptionResolver, null) {
		}

		#endregion

		protected ILogger Logger { get; }

		protected virtual WebhookFilterRequest BuildFilterRequest(IWebhookSubscription subscription) {
			return WebookFilterRequestFactory.CreateRequest(subscription);
		}

		protected virtual async Task<object> GetWebhookDataAsync(EventInfo eventInfo, CancellationToken cancellationToken) {
			Logger.LogDebug("GetWebhookDataAsync: getting data for an event");

			if (dataStrategy != null) {
				Logger.LogDebug("GetWebhookDataAsync: the data strategy was enabled");

				var factory = dataStrategy.GetDataFactory(eventInfo);
				if (factory != null) {
					Logger.LogDebug("GetWebhookDataAsync: using a factory for the event of type {EventType} to generate the webhook data",
						eventInfo.EventType);

					return await factory.CreateDataAsync(eventInfo, cancellationToken);
				}
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

			if (filterSelector == null) {
				Logger.LogTrace("None webhook filter was set: matching all filters to true");
				return true;
			}

			Logger.LogTrace("Selecting the filter evaluator for '{FilterFormat}' format", filterRequest.FilterFormat);

			var filterEvaluator = filterSelector.GetEvaluator(filterRequest.FilterFormat);

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

		private void TraceDeliveryResult(WebhookDeliveryResult deliveryResult) {
			if (!Logger.IsEnabled(LogLevel.Trace))
				return;

			if (!deliveryResult.HasAttempted) {
				Logger.LogTrace("The delivery was not attempted");
			} else if (deliveryResult.Successful) {
				Logger.LogTrace("The delivery was successful after {AttemptCount} attempts", deliveryResult.Attempts.Count());
			} else {
				Logger.LogTrace("The delivery failed after {AttemptCount} attempts", deliveryResult.Attempts.Count());
			}

			if (deliveryResult.HasAttempted) {
				int offset = 1;
				foreach (var attempt in deliveryResult.Attempts) {
					Logger.LogTrace("Attempt {AttemptNumber} - [{StartDate} - {EndDate}] {StatusCode}",
						offset, attempt.StartedAt, attempt.EndedAt, attempt.ResponseStatusCode);
					offset++;
				}
			}
		}

		public virtual async Task<WebhookNotificationResult> NotifyAsync(string tenantId, EventInfo eventInfo, CancellationToken cancellationToken) {
			var result = new WebhookNotificationResult();

			try {
				var subscriptions = await subscriptionResolver.ResolveSubscriptionsAsync(tenantId, eventInfo.EventType, true, cancellationToken);

				if (subscriptions == null || subscriptions.Count == 0) {
					Logger.LogTrace("No active subscription to event {EventType} found for Tenant {TenantId}", eventInfo.EventType, tenantId);
					return result;
				}

				foreach (var subscription in subscriptions) {
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

						result.AddDelivery(new WebhookDeliveryResult(webhook));
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
