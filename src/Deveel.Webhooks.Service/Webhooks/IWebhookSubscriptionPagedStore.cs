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
	/// <summary>
	/// Provides a contract for a store of webhook subscriptions that can be paged.
	/// </summary>
	/// <typeparam name="TSubscription">
	/// The type of webhook subscription that is handled by the store.
	/// </typeparam>
	public interface IWebhookSubscriptionPagedStore<TSubscription> : IWebhookSubscriptionStore<TSubscription> where TSubscription : class, IWebhookSubscription {
		/// <summary>
		/// Gets a page of subscriptions that match the given query.
		/// </summary>
		/// <param name="query">
		/// The query to execute to get the page of subscriptions.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a page of subscriptions that match the given query.
		/// </returns>
		Task<PagedResult<TSubscription>> GetPageAsync(PagedQuery<TSubscription> query, CancellationToken cancellationToken);
	}
}
