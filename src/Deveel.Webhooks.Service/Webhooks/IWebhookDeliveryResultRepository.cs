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
using Deveel.Data;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides a contract to store of webhook delivery results
	/// </summary>
	/// <typeparam name="TResult">
	/// The type of the result of the delivery of a webhook
	/// </typeparam>
	public interface IWebhookDeliveryResultRepository<TResult> : IRepository<TResult> where TResult : class, IWebhookDeliveryResult {
		/// <summary>
		/// Finds a single delivery result by the identifier of the webhook
		/// that was set during the notification process.
		/// </summary>
		/// <param name="webhookId">
		/// The unique identifier of the webhook (<see cref="IWebhook.Id"/>)
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token to cancel the operation
		/// </param>
		/// <returns>
		/// Returns the instance of the result that is associated with the given webhook,
		/// or <c>null</c> if not found
		/// </returns>
		Task<TResult?> FindByWebhookIdAsync(string webhookId, CancellationToken cancellationToken);
	}
}
