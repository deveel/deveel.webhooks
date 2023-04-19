using System;

using Microsoft.Extensions.Logging;

namespace Deveel.Webhooks {
    static partial class LoggerExtensions {
        [LoggerMessage(EventId = -20222, Level = LogLevel.Warning,
            Message = "It was not possible to resolve any webhook receiver for the type '{WebhookType}'")]
        public static partial void WarnReceiverNotRegistered(this ILogger logger, Type webhookType);
    }
}
