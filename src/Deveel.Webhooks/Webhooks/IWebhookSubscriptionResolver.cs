﻿// Copyright 2022 Deveel
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
	/// <summary>
	/// Provides a contract for the resolution of subscriptions to events.
	/// </summary>
	public interface IWebhookSubscriptionResolver {
		/// <summary>
		/// Resolves all the subscriptions to an event in the scope
		/// of a given tenant.
		/// </summary>
		/// <param name="tenantId">The identifier of the tenant owning
		/// the subscriptions.</param>
		/// <param name="eventType">The type of event that occurred.</param>
		/// <param name="activeOnly">A flag indicating whether only active
		/// subscriptions should be returned.</param>
		/// <param name="cancellationToken"></param>
		/// <returns>
		/// Returns a list of <see cref="IWebhookSubscription"/> instances
		/// that are matching the given basic conditions.
		/// </returns>
		Task<IList<IWebhookSubscription>> ResolveSubscriptionsAsync(string tenantId, string eventType, bool activeOnly, CancellationToken cancellationToken);
	}
}
