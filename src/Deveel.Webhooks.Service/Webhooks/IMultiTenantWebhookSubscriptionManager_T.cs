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
using System.Threading.Tasks;
using System.Threading;

namespace Deveel.Webhooks {
	public interface IMultiTenantWebhookSubscriptionManager<TSubscription> 
		where TSubscription : class, IWebhookSubscription {
		Task<string> AddSubscriptionAsync(string tenantId, WebhookSubscriptionInfo subscription, CancellationToken cancellationToken);

		Task<string> AddSubscriptionAsync(string tenantId, TSubscription subscription, CancellationToken cancellationToken);

		Task<bool> EnableSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken);

		Task<bool> EnableSubscriptionAsync(string tenantId, TSubscription subscription, CancellationToken cancellationToken);

		Task<bool> DisableSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken);

		Task<bool> DisableSubscriptionAsync(string tenantId, TSubscription subscription, CancellationToken cancellationToken);

		Task<TSubscription> GetSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken);

		Task<PagedResult<TSubscription>> GetSubscriptionsAsync(string tenantId, PagedQuery<TSubscription> query, CancellationToken cancellationToken);

		Task<bool> RemoveSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken);

		Task<bool> RemoveSubscriptionAsync(string tenantId, TSubscription subscription, CancellationToken cancellationToken);

		Task<int> CountAllAsync(string tenantId, CancellationToken cancellationToken);
	}
}
