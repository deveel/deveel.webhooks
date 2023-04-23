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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using MongoFramework;

namespace Deveel.Webhooks {
	public sealed class MongoDbWebhookStorageBuilder<TSubscription> where TSubscription : MongoWebhookSubscription {
		private readonly WebhookSubscriptionBuilder<TSubscription> builder;

		internal MongoDbWebhookStorageBuilder(WebhookSubscriptionBuilder<TSubscription> builder) {
			this.builder = builder;

			AddDefaultStorage();
		}

		private IServiceCollection Services => builder.Services;

		private void AddDefaultStorage() {
			Services.TryAddSingleton<IWebhookSubscriptionStore<MongoWebhookSubscription>, MongoDbWebhookSubscriptionStrore>();
			Services.AddSingleton<MongoDbWebhookSubscriptionStrore>();
			Services.TryAddSingleton<MongoDbWebhookSubscriptionStrore<MongoWebhookSubscription>>();
			Services.TryAddSingleton<IWebhookSubscriptionStoreProvider<MongoWebhookSubscription>, MongoDbWebhookSubscriptionStoreProvider>();
			Services.AddSingleton<MongoDbWebhookSubscriptionStoreProvider>();
			Services.TryAddSingleton<MongoDbWebhookSubscriptionStoreProvider<MongoWebhookSubscription>>();


			Services.TryAddSingleton<IWebhookDeliveryResultStore<MongoWebhookDeliveryResult>, MongoDbWebhookDeliveryResultStore>();
			Services.AddSingleton<MongoDbWebhookDeliveryResultStore>();
			Services.TryAddSingleton<MongoDbWebhookDeliveryResultStore<MongoWebhookDeliveryResult>>();
			Services.TryAddSingleton<IWebhookDeliveryResultStoreProvider<MongoWebhookDeliveryResult>, MongoDbWebhookDeliveryResultStoreProvider>();
			Services.AddSingleton<MongoDbWebhookDeliveryResultStoreProvider>();
			Services.TryAddSingleton<MongoDbWebhookDeliveryResultStoreProvider<MongoWebhookDeliveryResult>>();
		}

		public MongoDbWebhookStorageBuilder<TSubscription> Configure(string sectionPath) {
			Services.AddSingleton<IMongoDbWebhookContext>(provider => {
				var configuration = provider.GetRequiredService<IConfiguration>();
				var section = configuration.GetSection(sectionPath);
				var connectionString = section.GetValue<string>("ConnectionString");
				var connection = MongoDbConnection.FromConnectionString(connectionString);

				return new MongoDbWebhookContext(connection);
			});

			return this;
		}

		public MongoDbWebhookStorageBuilder<TSubscription> WithConnectionString(string connectionString) {
			Services.AddSingleton<IMongoDbWebhookContext>(provider => {
				var connection = MongoDbConnection.FromConnectionString(connectionString);
				return new MongoDbWebhookContext(connection);
			});

			return this;
		}

		public MongoDbWebhookStorageBuilder<TSubscription> WithConnectionStringName(string connectionStringName) {
			Services.AddSingleton<IMongoDbWebhookContext>(provider => {
				var configuration = provider.GetRequiredService<IConfiguration>();
				var connectionString = configuration.GetConnectionString(connectionStringName);
				var connection = MongoDbConnection.FromConnectionString(connectionString);
				return new MongoDbWebhookContext(connection);
			});

			return this;
		}


		public MongoDbWebhookStorageBuilder<TSubscription> UseMultiTenant() {
			Services.AddSingleton<IMongoDbWebhookContext, MongoDbWebhookTenantContext>();

			return this;
		}

		public MongoDbWebhookStorageBuilder<TSubscription> UseSubscriptionStore<TStore>()
			where TStore : MongoDbWebhookSubscriptionStrore {
			Services.AddSingleton<IWebhookSubscriptionStore<MongoWebhookSubscription>, TStore>();

			return this;
		}

		public MongoDbWebhookStorageBuilder<TSubscription> UseDeliveryResultStore<TStore>()
			where TStore : MongoDbWebhookDeliveryResultStore {
			Services.AddSingleton<IWebhookDeliveryResultStore<MongoWebhookDeliveryResult>, TStore>();

			return this;
		}

		public MongoDbWebhookStorageBuilder<TSubscription> UseDeliveryResultLogger<TWebhook, TResult>()
			where TWebhook : class, IWebhook
			where TResult : MongoWebhookDeliveryResult, new() {

			Services.AddSingleton<IWebhookDeliveryResultLogger<TWebhook>, MongoDbWebhookDeliveryResultLogger<TWebhook, TResult>>();

			return this;
		}

		public MongoDbWebhookStorageBuilder<TSubscription> UseDeliveryResultLogger()
			=> UseDeliveryResultLogger<Webhook, MongoWebhookDeliveryResult>();
	}
}
