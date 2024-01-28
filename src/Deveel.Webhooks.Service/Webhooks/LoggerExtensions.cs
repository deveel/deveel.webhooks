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

namespace Deveel.Webhooks {
	static partial class LoggerExtensions {
		[LoggerMessage(LoggerEventIds.UnknownError, LogLevel.Error, "An unknown error occurred while managing webhook subscriptions")]
		public static partial void LogUnknownError(this ILogger logger, Exception exception);

		[LoggerMessage(LoggerEventIds.UnknownSubscriptionError, LogLevel.Error, "An unknown error occurred while managing the webhook subscription {SubscriptionId}")]
		public static partial void LogUnknownSubscriptionError(this ILogger logger, Exception exception, string? subscriptionId);
	}
}
