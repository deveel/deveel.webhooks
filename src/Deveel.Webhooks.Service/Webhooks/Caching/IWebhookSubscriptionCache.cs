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
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks.Caching {
	/// <summary>
	/// A cache of webhook subscriptions that optimizes the
	/// read access to the entities
	/// </summary>
	/// <typeparam name="TSubscription"></typeparam>
	public interface IWebhookSubscriptionCache {
		Task<IWebhookSubscription> GetByIdAsync(string tenantId, string id, CancellationToken cancellationToken);

		Task<IList<IWebhookSubscription>> GetByEventTypeAsync(string tenantId, string eventType, CancellationToken cancellationToken);

		Task SetAsync(IWebhookSubscription subscription, CancellationToken cancellationToken);

		Task RemoveAsync(IWebhookSubscription subscription, CancellationToken cancellationToken);
	}
}
