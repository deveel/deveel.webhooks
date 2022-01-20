// Copyright 2022 Deveel
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
using Deveel.Webhooks.Storage;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Webhooks {
	public static class WebhookServiceBuilderExtensions {
		private static IServiceCollection ConfigureMongoDbOptions(this IServiceCollection services, string sectionName, string connectionStringName = null) {
			services.AddOptions<MongoDbOptions>()
				.Configure<IConfiguration>((options, config) => {
					var section = config.GetSection(sectionName);
					if (section != null)
						section.Bind(options);

					if (!String.IsNullOrWhiteSpace(connectionStringName)) {
						options.ConnectionString = config.GetConnectionString(connectionStringName);
					}
				});

			return services;
		}

		private static IWebhookServiceBuilder UseMongoDb(this IWebhookServiceBuilder builder) {
			return builder
				.Use<IWebhookSubscriptionFactory, MongoDbWebhookSubscriptionFactory>()
				.Use<IWebhookSubscriptionStore, MongoDbWebhookSubscriptionStrore>()
				.Use<IWebhookSubscriptionStore<WebhookSubscriptionDocument>, MongoDbWebhookSubscriptionStrore>()
				.Use<IWebhookSubscriptionStoreProvider, MongoDbWebhookSubscriptionStoreProvider>()
				.Use<IWebhookSubscriptionStoreProvider<WebhookSubscriptionDocument>, MongoDbWebhookSubscriptionStoreProvider>();
		}

		public static IWebhookServiceBuilder UseMongoDb(this IWebhookServiceBuilder builder, string sectionName, string connectionStringName = null) {
			builder.ConfigureServices(services => services.ConfigureMongoDbOptions(sectionName, connectionStringName));
			builder.UseMongoDb();
			return builder;
		}


		public static IWebhookServiceBuilder UseMongoDb(this IWebhookServiceBuilder builder, Action<MongoDbOptions> configure) {
			builder.ConfigureServices(services => services.AddOptions<MongoDbOptions>().Configure(configure));

			builder.UseMongoDb();
			return builder;
		}

		public static IWebhookServiceBuilder UseMongoDb(this IWebhookServiceBuilder builder, Action<IMongoDbOptionBuilder> configure) {
			builder.ConfigureServices(services => {
				services.AddOptions<MongoDbOptions>()
				.Configure(options => {
					var optionsBuilder = new MongoDbOptionBuilder(options);
					configure(optionsBuilder);
				});
			});

			return builder;
		}
	}
}
