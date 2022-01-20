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

namespace Deveel.Webhooks.Storage {
	public static class WebhookSubscriptionStoreExtensions {
		public static Task<string> CreateAsync<TSubscription>(this IWebhookSubscriptionStoreProvider<TSubscription> provider, string tenantId,
			TSubscription subscription, CancellationToken cancellationToken = default)
			where TSubscription : class, IWebhookSubscription
			=> provider.GetTenantStore(tenantId).CreateAsync(subscription, cancellationToken);

		public static Task<bool> DeleteAsync<TSubscription>(this IWebhookSubscriptionStoreProvider<TSubscription> provider, string tenantId,
			TSubscription subscription, CancellationToken cancellationToken = default)
			where TSubscription : class, IWebhookSubscription
			=> provider.GetTenantStore(tenantId).DeleteAsync(subscription, cancellationToken);

		public static Task<bool> UpdateAsync<TSubscription>(this IWebhookSubscriptionStoreProvider<TSubscription> provider, string tenantId,
			TSubscription subscription, CancellationToken cancellationToken = default)
			where TSubscription : class, IWebhookSubscription
			=> provider.GetTenantStore(tenantId).UpdateAsync(subscription, cancellationToken);

		public static Task<TSubscription> FindByIdAsync<TSubscription>(this IWebhookSubscriptionStoreProvider<TSubscription> provider, string tenantId,
			string id, CancellationToken cancellationToken = default)
			where TSubscription : class, IWebhookSubscription
			=> provider.GetTenantStore(tenantId).FindByIdAsync(id, cancellationToken);

		public static Task<IList<TSubscription>> GetByEventTypeAsync<TSubscription>(this IWebhookSubscriptionStoreProvider<TSubscription> provider, string tenantId,
			string eventType, bool activeOnly, CancellationToken cancellationToken = default)
			where TSubscription : class, IWebhookSubscription
			=> provider.GetTenantStore(tenantId).GetByEventTypeAsync(eventType, activeOnly, cancellationToken);

		public static Task SetStateAsync<TSubscripton>(this IWebhookSubscriptionStoreProvider<TSubscripton> provider, string tenantId, TSubscripton subscripton, WebhookSubscriptionStateInfo stateInfo, CancellationToken cancellationToken = default)
			where TSubscripton : class, IWebhookSubscription
			=> provider.GetTenantStore(tenantId).SetStateAsync(subscripton, stateInfo, cancellationToken);

		public static Task<int> CountAllAsync<TSubscription>(this IWebhookSubscriptionStoreProvider<TSubscription> provider, string tenantId, CancellationToken cancellationToken = default)
			where TSubscription : class, IWebhookSubscription
			=> provider.GetTenantStore(tenantId).CountAllAsync(cancellationToken);
	}
}
