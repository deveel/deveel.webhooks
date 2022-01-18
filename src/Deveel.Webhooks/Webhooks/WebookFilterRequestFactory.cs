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
using System.Linq;

namespace Deveel.Webhooks {
	static class WebookFilterRequestFactory {
		public static WebhookFilterRequest CreateRequest(IWebhookSubscription subscription) {
			if (subscription.Filters == null ||
				!subscription.Filters.Any())
				return null;

			var formats = subscription.Filters?.Select(x => x.Format).Distinct().ToList();
			if (formats.Count > 1)
				throw new InvalidOperationException("The subscription has filters with multiple formats");

			var request = new WebhookFilterRequest(formats[0]);

			foreach (var filter in subscription.Filters) {
				request.AddFilter(filter.Expression);
			}

			return request;
		}
	}
}
