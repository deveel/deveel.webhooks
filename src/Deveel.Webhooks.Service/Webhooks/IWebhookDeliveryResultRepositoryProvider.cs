// Copyright 2022-2024 Antonello Provenzano
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

using Deveel.Data;

namespace Deveel.Webhooks {
	/// <summary>
	/// A service that provides a resolution mechanism for stores 
	/// of webhook delivery results for a given tenant.
	/// </summary>
	/// <typeparam name="TResult">
	/// The type of the result of the delivery of a webhook.
	/// </typeparam>
	public interface IWebhookDeliveryResultRepositoryProvider<TResult> : IRepositoryProvider<TResult> 
		where TResult : class, IWebhookDeliveryResult {
		/// <summary>
		/// Gets the repository of delivery results for the given tenant.
		/// </summary>
		/// <param name="tenantId">
		/// The identifier of the tenant owning the repository.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns the repository of webhook delivery results for 
		/// the given tenant.
		/// </returns>
		/// <exception cref="WebhookServiceException">
		/// Thrown if the repository cannot be resolved for the given tenant.
		/// </exception>
		new Task<IWebhookDeliveryResultRepository<TResult>> GetRepositoryAsync(string tenantId, CancellationToken cancellationToken = default);
	}
}
