using Microsoft.Extensions.Logging;

namespace Deveel.Webhooks {
	static partial class LoggerExtensions {
		[LoggerMessage(LoggerEventIds.UnknownError, LogLevel.Error, "An unknown error occurred while managing webhook subscriptions")]
		public static partial void LogUnknownError(this ILogger logger, Exception exception);

		[LoggerMessage(LoggerEventIds.UnknownSubscriptionError, LogLevel.Error, "An unknown error occurred while managing the webhook subscription {SubscriptionId}")]
		public static partial void LogUnknownSubscriptionError(this ILogger logger, Exception exception, string? subscriptionId);
	}
}
