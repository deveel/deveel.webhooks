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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public interface IWebhookSubscriptionStore<TSubscription> : IDisposable
		where TSubscription : class, IWebhookSubscription {


		Task<string> CreateAsync(TSubscription subscription, CancellationToken cancellationToken);

		Task<TSubscription> FindByIdAsync(string id, CancellationToken cancellationToken);

		Task<bool> DeleteAsync(TSubscription subscription, CancellationToken cancellationToken);

		Task<bool> UpdateAsync(TSubscription subscription, CancellationToken cancellationToken);

		Task<int> CountAllAsync(CancellationToken cancellationToken);


		Task<IList<TSubscription>> GetByEventTypeAsync(string eventType, bool activeOnly, CancellationToken cancellationToken);

		Task SetStateAsync(TSubscription subscription, WebhookSubscriptionStatus status, CancellationToken cancellationToken);
	}
}