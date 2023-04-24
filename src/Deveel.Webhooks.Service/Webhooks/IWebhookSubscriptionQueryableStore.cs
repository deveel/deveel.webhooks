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
using System.Linq;

namespace Deveel.Webhooks {
	/// <summary>
	/// A store for <see cref="IWebhookSubscription"/> that can be queried.
	/// </summary>
	/// <typeparam name="TSubscription"></typeparam>
	public interface IWebhookSubscriptionQueryableStore<TSubscription> : IWebhookSubscriptionStore<TSubscription>
		where TSubscription : class, IWebhookSubscription {
		/// <summary>
		/// Gets a queryable object that can be used to query the
		/// subscriptions in the store.
		/// </summary>
		/// <returns>
		/// Returns a <see cref="IQueryable{T}"/> object that can be used
		/// to query the subscriptions in the store.
		/// </returns>
		IQueryable<TSubscription> AsQueryable();
	}
}
