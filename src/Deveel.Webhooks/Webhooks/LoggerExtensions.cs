// Copyright 2022-2025 Antonello Provenzano
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

namespace Deveel.Webhooks {
	static partial class LoggerExtensions {
		[LoggerMessage(EventId = -10002, Level = LogLevel.Error,
				Message = "An error occurred while trying to deliver a webhook to the subscription {SubscriptionId} and event {EventType}")]
		public static partial void LogUnknownEventDeliveryError(this ILogger logger, Exception ex, string subscriptionId, string eventType);

		[LoggerMessage(EventId = -10001, Level = LogLevel.Error,
		Message = "An error occurred while trying to deliver a webhook to the subscription {SubscriptionId}")]
		public static partial void LogUnknownDeliveryError(this ILogger logger, Exception ex, string subscriptionId);


		[LoggerMessage(EventId = - 10003, Level = LogLevel.Error,
			Message = "Error while creating the webhook for the subscription {SubscriptionId} and event {EventType}")]
		public static partial void LogWebhookCreationError(this ILogger logger, Exception ex, string subscriptionId, string eventType);

		[LoggerMessage(EventId = 100001, Level = LogLevel.Trace, 
			Message = "The filter is empty - matching all subscriptions")]
		public static partial void TraceEmptyFilter(this ILogger logger);

		[LoggerMessage(EventId = 100003, Level = LogLevel.Trace, 
			Message = "The filter is a wildcard - matching all subscriptions")]
		public static partial void TraceWildcardFilter(this ILogger logger);

		[LoggerMessage(EventId = 100004, Level = LogLevel.Debug,
			Message = "Selecting an evaluator for filter of format '{FilterFormat}'")]
		public static partial void TraceSelectingEvaluator(this ILogger logger, string filterFormat);

		[LoggerMessage(EventId = 100005, Level = LogLevel.Warning,
						Message = "The evaluator for filter of format '{FilterFormat}' was not found")]
		public static partial void WarnEvaluatorNotFound(this ILogger logger, string filterFormat);

		[LoggerMessage(EventId = 100002, Level = LogLevel.Debug, 
									Message = "Evaluating the subscription {SubscriptionId} for event {EventType}")]
		public static partial void TraceEvaluatingSubscription(this ILogger logger, string subscriptionId, string eventType);

		[LoggerMessage(EventId = 100023, Level = LogLevel.Debug, 
			Message = "Subscription {SubscriptionId} matched the event {EventType}")]
		public static partial void TraceSubscriptionMatched(this ILogger logger, string subscriptionId, string eventType);

		[LoggerMessage(EventId = 100024, Level = LogLevel.Debug, 
			Message = "Subscription {SubscriptionId} did not match the event {EventType}")]
		public static partial void TraceSubscriptionNotMatched(this ILogger logger, string subscriptionId, string eventType);

		[LoggerMessage(EventId = -100200, Level = LogLevel.Warning,
			Message = "The webhook was not created for the subscription {SubscriptionId} and event {EventType}")]
		public static partial void WarnWebhookNotCreated(this ILogger logger, string subscriptionId, string eventType);

		[LoggerMessage(EventId = 100200, Level = LogLevel.Warning,
			Message = "No attempts to deliver the webhook for event {EventType} to {DestinationUrl} were done")]
		public static partial void WarnDeliveryNotAttempted(this ILogger logger, string destinationUrl, string eventType);

		[LoggerMessage(EventId = 100201, Level = LogLevel.Debug,
			Message = "The webhook for event {EventType} was delivered to {DestinationUrl} after {AttemptCount} attempts")]
		public static partial void TraceDeliveryDoneAfterAttempts(this ILogger logger, string destinationUrl, string eventType, int attemptCount);

		[LoggerMessage(EventId = -100202, Level = LogLevel.Warning,
			Message = "The webhook for event {EventType} failed to deliver to {DestinationUrl} after {AttemptCount} attempts")]
		public static partial void WarnDeliveryFailed(this ILogger logger, string destinationUrl, string eventType, int attemptCount);

		[LoggerMessage(EventId = 100202, Level = LogLevel.Trace,
			Message = "Attempt {AttemptNumber} successfull - started at {StartedAt}, completed at {CompletedAt}, responseCode {ResponseCode}")]
		public static partial void TraceDeliveryAttemptSuccess(this ILogger logger, int attemptNumber, DateTimeOffset startedAt, DateTimeOffset? completedAt, int? responseCode);

		[LoggerMessage(EventId = 100203, Level = LogLevel.Trace,
			Message = "Attempt {AttemptNumber} failed - started at {StartedAt}, completed at {CompletedAt}, responseCode {ResponseCode}")]
		public static partial void TraceDeliveryAttemptFailed(this ILogger logger, int attemptNumber, DateTimeOffset startedAt, DateTimeOffset? completedAt, int? responseCode);

	}
}
