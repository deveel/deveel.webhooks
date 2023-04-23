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
	/// Provides a contract for a store that can retrieve pages 
	/// of webhook delivery
	/// </summary>
	/// <typeparam name="TResult">
	/// The type of the result of the delivery
	/// </typeparam>
	/// <seealso cref="IWebhookDeliveryResultStore{TResult}"/>
	public interface IWebhookDeliveryResultPagedStore<TResult> : IWebhookDeliveryResultStore<TResult> where TResult : class, IWebhookDeliveryResult {
		/// <summary>
		/// Gets a page of webhook delivery results from the store
		/// </summary>
		/// <param name="query">
		/// The query that defines the page to retrieve
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token to cancel the operation
		/// </param>
		/// <returns>
		/// Returns a <see cref="PagedResult{TItem}"/> that contains the
		/// items in the page and the total number of results in the store
		/// for the given query.
		/// </returns>
		Task<PagedResult<TResult>> GetPageAsync(PagedQuery<TResult> query, CancellationToken cancellationToken);
	}
}
