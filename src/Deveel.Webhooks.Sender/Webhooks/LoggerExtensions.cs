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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace Deveel.Webhooks {
	static partial class LoggerExtensions {
		[LoggerMessage(EventId = -100010, Level = LogLevel.Error, 
						Message = "An unknown error occurred")]
		public static partial void LogUnknownError(this ILogger logger, Exception error);

		[LoggerMessage(EventId = 120300, Level = LogLevel.Debug, 
									Message = "Creating a {Algorithm} signature")]
		public static partial void TraceCreatingSignature(this ILogger logger, string algorithm);

		[LoggerMessage(EventId = 120301, Level = LogLevel.Debug, 
												Message = "A {Algorithm} signature has been created")]
		public static partial void TraceSignatureCreated(this ILogger logger, string algorithm);

		[LoggerMessage(EventId = 120302, Level = LogLevel.Debug, 
			Message = "Serializing the webhook to '{Format}'")]
		public static partial void TraceSerializing(this ILogger logger, string format);

		[LoggerMessage(EventId = 120303, Level = LogLevel.Debug, 
						Message = "The webhook has been serialized to '{Format}'")]
		public static partial void TraceSerialized(this ILogger logger, string format);

		[LoggerMessage(EventId = 120304, Level = LogLevel.Debug, 
						Message = "Signing the webhook with '{Algorithm}' on {Location} '{ParameterName}'")]
		public static partial void TraceSigningRequest(this ILogger logger, string algorithm, WebhookSignatureLocation location, string parameterName);

		[LoggerMessage(EventId = 120305, Level = LogLevel.Debug, 
									Message = "The webhook has been signed with '{Algorithm}' on {Location} '{ParameterName}'")]
		public static partial void TraceRequestSigned(this ILogger logger, string algorithm, WebhookSignatureLocation location, string parameterName);

		[LoggerMessage(EventId = 120306, Level = LogLevel.Debug, 
									Message = "Attempting to send a webhook to '{DestinationUrl}'")]
		public static partial void TraceStartingAttempt(this ILogger logger, string destinationUrl);

		[LoggerMessage(EventId = 120307, Level = LogLevel.Debug, 
			Message = "The delivery attempt to '{DestinationUrl}' finished with status code {StatusCode}")]
		public static partial void TraceAttemptFinished(this ILogger logger, string destinationUrl, int? statusCode);

		[LoggerMessage(EventId = -120311, Level = LogLevel.Warning, 
			Message = "Timeout while sending the webhook")]
		public static partial void WarnTimeOut(this ILogger logger, Exception error);

		[LoggerMessage(EventId = -120312, Level = LogLevel.Warning, 
						Message = "The request to '{DestinationUrl}' failed with status code {StatusCode}")]
		public static partial void WarnRequestFailed(this ILogger logger, Exception error, string destinationUrl, int? statusCode);

		[LoggerMessage(EventId = 120327, Level = LogLevel.Debug, 
			Message = "Sending a webhook to the receiver '{DestinationUrl}'")]
		public static partial void TraceSendingWebhook(this ILogger logger, string destinationUrl);

		[LoggerMessage(EventId = 120328, Level = LogLevel.Debug, 
			Message = "The webhook has been sent to the receiver '{DestinationUrl}'")]
		public static partial void TraceSuccessfulDelivery(this ILogger logger, string destinationUrl);

		[LoggerMessage(EventId = -120329, Level = LogLevel.Debug, 
			Message = "The webhook has failed to be delivered to the receiver '{DestinationUrl}'")]
		public static partial void WarnDeliveryFailed(this ILogger logger, string destinationUrl);

		[LoggerMessage(EventId = 120330, Level = LogLevel.Debug, 
			Message = "The webhook has been delivered to the receiver '{DestinationUrl}'")]
		public static partial void TraceDeliveryFinished(this ILogger logger, string destinationUrl);

		[LoggerMessage(EventId = 120331, Level = LogLevel.Debug, 
			Message = "Verifying the the receiver '{VerificationUrl}' for the delivery")]
		public static partial void TraceVerifyingReceiver(this ILogger logger, string verificationUrl);

		[LoggerMessage(EventId = 120332, Level = LogLevel.Debug, 
			Message = "The receiver '{VerificationUrl}' has been verified for the delivery")]
		public static partial void TraceReceiverVerified(this ILogger logger, string verificationUrl);

		[LoggerMessage(EventId = -120333, Level = LogLevel.Error, 
			Message = "The receiver '{VerificationUrl}' has failed to be verified for the delivery")]
		public static partial void LogVerificationFailed(this ILogger logger, Exception error, string verificationUrl);
	}
}
