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

using Deveel.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Deveel.Webhooks {
	public sealed class MongoDbWebhookStorageBuilder<TSubscription> where TSubscription : MongoDbWebhookSubscription {
		private readonly WebhookSubscriptionBuilder<TSubscription> builder;

		internal MongoDbWebhookStorageBuilder(WebhookSubscriptionBuilder<TSubscription> builder) {
			this.builder = builder;

			AddDefaultStorage();
		}

		private IServiceCollection Services => builder.Services;

		private IServiceCollection AddSubscriptionFactory(IServiceCollection services) {
			services.TryAddSingleton<IWebhookSubscriptionFactory<TSubscription>, MongoDbWebhookSubscriptionFactory<TSubscription>>();
			services.TryAddSingleton<IWebhookSubscriptionFactory<MongoDbWebhookSubscription>, MongoDbWebhookSubscriptionFactory>();
			services.AddSingleton<MongoDbWebhookSubscriptionFactory>();

			return services;
		}

		private void AddDefaultStorage() {
			AddSubscriptionFactory(Services);

			Services.TryAddSingleton<IWebhookSubscriptionStore<MongoDbWebhookSubscription>, MongoDbWebhookSubscriptionStrore>();
			Services.AddSingleton<MongoDbWebhookSubscriptionStrore>();
			Services.TryAddSingleton<MongoDbWebhookSubscriptionStrore<MongoDbWebhookSubscription>>();
			Services.TryAddSingleton<IWebhookSubscriptionStoreProvider<MongoDbWebhookSubscription>, MongoDbWebhookSubscriptionStoreProvider>();
			Services.AddSingleton<MongoDbWebhookSubscriptionStoreProvider>();
			Services.TryAddSingleton<MongoDbWebhookSubscriptionStoreProvider<MongoDbWebhookSubscription>>();


			Services.TryAddSingleton<IWebhookDeliveryResultStore<MongoDbWebhookDeliveryResult>, MongoDbWebhookDeliveryResultStore>();
			Services.AddSingleton<MongoDbWebhookDeliveryResultStore>();
			Services.TryAddSingleton<MongoDbWebhookDeliveryResultStore<MongoDbWebhookDeliveryResult>>();
			Services.TryAddSingleton<IWebhookDeliveryResultStoreProvider<MongoDbWebhookDeliveryResult>, MongoDbWebhookDeliveryResultStoreProvider>();
			Services.AddSingleton<MongoDbWebhookDeliveryResultStoreProvider>();
			Services.TryAddSingleton<MongoDbWebhookDeliveryResultStoreProvider<MongoDbWebhookDeliveryResult>>();
		}

		public MongoDbWebhookStorageBuilder<TSubscription> Configure(Action<IMongoDbOptionBuilder> configure) {
			Services.AddOptions<MongoDbOptions>()
				.Configure(options => {
					var optionsBuilder = new MongoDbOptionBuilder(options);
					configure(optionsBuilder);
				});

			return this;
		}

		public MongoDbWebhookStorageBuilder<TSubscription> Configure(Action<MongoDbOptions> configure) {
			Services.AddOptions<MongoDbOptions>().Configure(configure);

			return this;
		}

		public MongoDbWebhookStorageBuilder<TSubscription> Configure(string sectionName, string connectionStringName = null) {
			Services.AddOptions<MongoDbOptions>()
				.Configure<IConfiguration>((options, config) => {
					var section = config.GetSection(sectionName);
					if (section != null)
						section.Bind(options);
					if (!string.IsNullOrWhiteSpace(connectionStringName))
						options.ConnectionString = config.GetConnectionString(connectionStringName);
				});

			return this;
		}

		public MongoDbWebhookStorageBuilder<TSubscription> UseSubscriptionStore<TStore>()
			where TStore : MongoDbWebhookSubscriptionStrore {
			AddSubscriptionFactory(Services);
			Services.AddSingleton<IWebhookSubscriptionStore<MongoDbWebhookSubscription>, TStore>();

			return this;
		}

		public MongoDbWebhookStorageBuilder<TSubscription> UseSubscriptionStoreProvider<TProvider>()
			where TProvider : MongoDbWebhookSubscriptionStoreProvider {
			AddSubscriptionFactory(Services);
			Services.AddSingleton<IWebhookSubscriptionStoreProvider<MongoDbWebhookSubscription>, TProvider>();

			return this;
		}

		public MongoDbWebhookStorageBuilder<TSubscription> UseDeliveryResultStore<TStore>()
			where TStore : MongoDbWebhookDeliveryResultStore {
			Services.AddSingleton<IWebhookDeliveryResultStore<MongoDbWebhookDeliveryResult>, TStore>();

			return this;
		}

		public MongoDbWebhookStorageBuilder<TSubscription> UseDeliveryResultStoreProvider<TProvider>()
			where TProvider : MongoDbWebhookDeliveryResultStoreProvider {
			AddSubscriptionFactory(Services);
			Services.AddSingleton<IWebhookDeliveryResultStoreProvider<MongoDbWebhookDeliveryResult>, TProvider>();

			return this;
		}

		public MongoDbWebhookStorageBuilder<TSubscription> UseDeliveryResultLogger<TWebhook, TResult>()
			where TWebhook : class, IWebhook
			where TResult : MongoDbWebhookDeliveryResult {

			Services.AddSingleton<IWebhookDeliveryResultLogger<TWebhook>, MongoDbWebhookDeliveryResultLogger<TWebhook, TResult>>();

			return this;
		}

		public MongoDbWebhookStorageBuilder<TSubscription> UseDeliveryResultLogger()
			=> UseDeliveryResultLogger<Webhook, MongoDbWebhookDeliveryResult>();
	}
}
