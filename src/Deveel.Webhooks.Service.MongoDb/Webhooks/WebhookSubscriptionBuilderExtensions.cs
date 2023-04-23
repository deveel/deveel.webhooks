﻿// Copyright 2022-2023 Deveel
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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Webhooks {
	public static class WebhookSubscriptionBuilderExtensions {
		public static MongoDbWebhookStorageBuilder<TSubscription> UseMongoDb<TSubscription>(this WebhookSubscriptionBuilder<TSubscription> builder)
			where TSubscription : MongoDbWebhookSubscription
			=> new MongoDbWebhookStorageBuilder<TSubscription>(builder);

		public static WebhookSubscriptionBuilder<TSubscription> UseMongoDb<TSubscription>(this WebhookSubscriptionBuilder<TSubscription> builder, Action<MongoDbWebhookStorageBuilder<TSubscription>> configure = null)
			where TSubscription : MongoDbWebhookSubscription {
			var storageBuilder = builder.UseMongoDb();
			configure?.Invoke(storageBuilder);

			return builder;
		}

		public static MongoDbWebhookStorageBuilder<TSubscription> UseMongoDb<TSubscription>(this WebhookSubscriptionBuilder<TSubscription> builder, string sectionName, string connectionStringName = null)
			where TSubscription : MongoDbWebhookSubscription
			=> builder.UseMongoDb().Configure(sectionName, connectionStringName);


		public static MongoDbWebhookStorageBuilder<TSubscription> UseMongoDb<TSubscription>(this WebhookSubscriptionBuilder<TSubscription> builder, Action<MongoDbOptions> configure)
			where TSubscription : MongoDbWebhookSubscription
			=> builder.UseMongoDb().Configure(configure);

		public static MongoDbWebhookStorageBuilder<TSubscription> UseMongoDb<TSubscription>(this WebhookSubscriptionBuilder<TSubscription> builder, Action<IMongoDbOptionBuilder> configure)
			where TSubscription : MongoDbWebhookSubscription
			=> builder.UseMongoDb().Configure(configure);
	}
}