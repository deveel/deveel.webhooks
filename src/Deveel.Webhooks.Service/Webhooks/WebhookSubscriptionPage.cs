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

namespace Deveel.Webhooks {
	public sealed class WebhookSubscriptionPage<TSubscription> where TSubscription : class, IWebhookSubscription {
		public WebhookSubscriptionPage(WebhookSubscriptionQuery<TSubscription> query, int totalCount, IEnumerable<TSubscription> subscriptions = null) {
			if (totalCount < 0)
				throw new ArgumentOutOfRangeException(nameof(totalCount), "The total count must be equal or greater than zero");

			Query = query ?? throw new ArgumentNullException(nameof(query));
			TotalCount = totalCount;
			Subscriptions = subscriptions;
		}

		public WebhookSubscriptionQuery<TSubscription> Query { get; }

		public IEnumerable<TSubscription> Subscriptions { get; set; }

		public int TotalCount { get; }

		public int PageSize => Query.PageSize;

		public int Offset => Query.Offset;

		public int TotalPages => (int)Math.Ceiling((double)TotalCount / Query.PageSize);
	}
}
