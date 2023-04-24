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
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	/// <summary>
	/// Represents a validator for a webhook subscription
	/// before creating or updating it.
	/// </summary>
	/// <typeparam name="TSubscription">
	/// The type of the subscription to validate.
	/// </typeparam>
	public interface IWebhookSubscriptionValidator<TSubscription> where TSubscription : class, IWebhookSubscription {
		/// <summary>
		/// Validates the given subscription.
		/// </summary>
		/// <param name="manager">
		/// The instance of the manager that is validating the subscription.
		/// </param>
		/// <param name="subscription">
		/// The subscription instance to validate.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a <see cref="WebhookValidationResult"/> that
		/// indicates if the subscription is valid or not.
		/// </returns>
		Task<WebhookValidationResult> ValidateAsync(WebhookSubscriptionManager<TSubscription> manager, TSubscription subscription, CancellationToken cancellationToken);
	}
}
