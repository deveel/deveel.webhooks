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

using Microsoft.Extensions.Logging;

namespace Deveel.Webhooks {
    static partial class LoggerExtensions {
        [LoggerMessage(EventId = -20222, Level = LogLevel.Warning,
            Message = "It was not possible to resolve any webhook receiver for the type '{WebhookType}'")]
        public static partial void WarnReceiverNotRegistered(this ILogger logger, Type webhookType);
    }
}
