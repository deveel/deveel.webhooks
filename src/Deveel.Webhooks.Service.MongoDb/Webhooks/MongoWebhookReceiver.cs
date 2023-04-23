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

// Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public class MongoWebhookReceiver : IWebhookReceiver {

		public string DestinationUrl { get; set; }

		IEnumerable<KeyValuePair<string, string>> IWebhookReceiver.Headers => Headers;

		public IDictionary<string, string> Headers { get; set; }

		public string BodyFormat { get; set; }

		public string? SubscriptionId { get; set; }

		public string? SubscriptionName { get; set; }
	}
}
