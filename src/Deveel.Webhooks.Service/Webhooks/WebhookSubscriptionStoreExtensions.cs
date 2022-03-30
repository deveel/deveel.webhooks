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
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public static class WebhookSubscriptionStoreExtensions {
		public static Task<string> CreateAsync<TSubscription>(this IWebhookSubscriptionStore<TSubscription> store, TSubscription entity)
			where TSubscription : class, IWebhookSubscription
			=> store.CreateAsync(entity, default);

		public static string Create<TSubscription>(this IWebhookSubscriptionStore<TSubscription> store, TSubscription entity)
			where TSubscription : class, IWebhookSubscription
			=> store.CreateAsync(entity, default)
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult();

		public static Task<TSubscription> FindByIdAsync<TSubscription>(this IWebhookSubscriptionStore<TSubscription> store, string id)
			where TSubscription : class, IWebhookSubscription
			=> store.FindByIdAsync(id, default);

		public static TSubscription FindById<TSubscription>(this IWebhookSubscriptionStore<TSubscription> store, string id)
			where TSubscription : class, IWebhookSubscription
			=> store.FindByIdAsync(id, default)
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult();
	}
}
