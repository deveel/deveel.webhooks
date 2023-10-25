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

using System.Diagnostics.CodeAnalysis;

namespace Deveel.Webhooks {
    public class DbWebhookReceiver : IWebhookReceiver {
        public int? Id { get; set; }

        public string? SubscriptionId { get; set; }

        public virtual DbWebhookSubscription? Subscription { get; set; }

        public string? SubscriptionName { get; set; }

        public string DestinationUrl { get; set; }

		[ExcludeFromCodeCoverage]
        IEnumerable<KeyValuePair<string, string>> IWebhookReceiver.Headers
            => Headers?.ToDictionary(x => x.Key, y => y.Value);

        public virtual List<DbWebhookReceiverHeader> Headers { get; set; }

        public string BodyFormat { get; set; }
    }
}
