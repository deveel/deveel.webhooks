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

namespace Deveel.Webhooks {
	public sealed class WebhookSubscriptionInfo {
		public WebhookSubscriptionInfo(string eventTypes, string destinationUrl)
			: this(new[] { eventTypes }, new Uri(destinationUrl)) {
		}

		public WebhookSubscriptionInfo(string[] eventTypes, string destinationUrl)
			: this(eventTypes, new Uri(destinationUrl)) {
		}

		public WebhookSubscriptionInfo(string eventType, Uri destinationUrl)
			: this(new[] { eventType }, destinationUrl) {
		}


		public WebhookSubscriptionInfo(string[] eventTypes, Uri destinationUrl) {
			if (eventTypes == null || eventTypes.Length == 0)
				throw new ArgumentException("At least one event type is required");
			if (eventTypes.Any(x => String.IsNullOrWhiteSpace(x)))
				throw new ArgumentException($"'{nameof(eventTypes)}' cannot contain null or whitespace entries.", nameof(eventTypes));

			EventTypes = eventTypes;
			DestinationUrl = destinationUrl;
		}

		public string Name { get; set; }

		public string[] EventTypes { get; }

		public Uri DestinationUrl { get; }

		public int RetryCount { get; set; }

		public string Secret { get; set; }

		public IEnumerable<IWebhookFilter> Filters { get; set; }

		/// <summary>
		/// The initial state of the subscription
		/// </summary>
		public bool Active { get; set; } = true;

		public IWebhookFilter Filter {
			get => Filters?.SingleOrDefault();
			set => Filters = new[] { value };
		}

		public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

		public IDictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
	}
}
