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

using Microsoft.Extensions.Logging;

namespace Deveel.Webhooks {
    static partial class LoggerExtensions {
		[LoggerMessage(EventId = 10001, Level = LogLevel.Debug, 
			Message = "A Webhook has arrived")]
		public static partial void TraceWebhookArrived(this ILogger logger);

		[LoggerMessage(EventId = 10002, Level = LogLevel.Debug, 
			Message = "A Webhook has been received")]
		public static partial void TraceWebhookReceived(this ILogger logger);

		[LoggerMessage(EventId = 10003, Level = LogLevel.Debug, 
			Message = "The webhook of has been handled by '{HandlerType}'")]
		public static partial void TraceWebhookHandled(this ILogger logger, Type handlerType);

		[LoggerMessage(EventId = 10004, Level = LogLevel.Debug, 
						Message = "A request of verification has arrived")]
		public static partial void TraceVerificationRequest(this ILogger logger);

		[LoggerMessage(EventId = 10005, Level = LogLevel.Debug, 
									Message = "The verification request has been completed successfully")]
		public static partial void TraceSuccessVerification(this ILogger logger);

		[LoggerMessage(EventId = -20226, Level = LogLevel.Warning, 
			Message = "The verification request has failed")]
		public static partial void WarnVerificationFailed(this ILogger logger);

        [LoggerMessage(EventId = -20222, Level = LogLevel.Warning,
            Message = "It was not possible to resolve any webhook receiver")]
        public static partial void WarnReceiverNotRegistered(this ILogger logger);

		[LoggerMessage(EventId = -20225, Level = LogLevel.Warning, 
			Message = "It was not possible to verify the signature of the webhook")]
		public static partial void WarnInvalidSignature(this ILogger logger);

		[LoggerMessage(EventId = -20224, Level = LogLevel.Warning, 
						Message = "The received webhook is invalid")]
		public static partial void WarnInvalidWebhook(this ILogger logger);

		[LoggerMessage(EventId = -20223, Level = LogLevel.Error,
						Message = "It was not possible to receive a webhook for an unhandled error")]
		public static partial void LogUnhandledReceiveError(this ILogger logger, Exception error);

		[LoggerMessage(EventId = -20227, Level = LogLevel.Error,
									Message = "Unhandled error while executing the handler of type '{HandlerType}' for webhooks of type '{WebhookType}'")]
		public static partial void LogUnhandledHandlerError(this ILogger logger, Exception error, Type handlerType, Type webhookType);
    }
}
