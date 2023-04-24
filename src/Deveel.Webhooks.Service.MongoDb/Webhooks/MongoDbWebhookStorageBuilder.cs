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

using Finbuckle.MultiTenant;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using MongoFramework;

namespace Deveel.Webhooks {
    /// <summary>
    /// Provides a builder to configure the MongoDB storage of webhook entities
    /// </summary>
    /// <typeparam name="TSubscription">
    /// The type of the subscription entity to store in the database.
    /// </typeparam>
    public sealed class MongoDbWebhookStorageBuilder<TSubscription> where TSubscription : MongoWebhookSubscription {
		private readonly WebhookSubscriptionBuilder<TSubscription> builder;

		internal MongoDbWebhookStorageBuilder(WebhookSubscriptionBuilder<TSubscription> builder) {
			this.builder = builder;

			AddDefaultStorage();
		}

		private IServiceCollection Services => builder.Services;

		private void AddDefaultStorage() {
			Services.TryAddSingleton<IMongoDbContext, MongoDbWebhookContext>();
			Services.TryAddSingleton<IMongoDbWebhookContext, MongoDbWebhookContext>();

			Services.TryAddSingleton<IWebhookSubscriptionStore<MongoWebhookSubscription>, MongoDbWebhookSubscriptionStrore>();
			Services.AddSingleton<MongoDbWebhookSubscriptionStrore>();
			Services.TryAddSingleton<MongoDbWebhookSubscriptionStrore<MongoWebhookSubscription>>();


			Services.TryAddSingleton<IWebhookDeliveryResultStore<MongoWebhookDeliveryResult>, MongoDbWebhookDeliveryResultStore>();
			Services.AddSingleton<MongoDbWebhookDeliveryResultStore>();
			Services.TryAddSingleton<MongoDbWebhookDeliveryResultStore<MongoWebhookDeliveryResult>>();
		}

		/// <summary>
		/// Configures the MongoDB storage context using a section
		/// from the application configuration.
		/// </summary>
		/// <param name="sectionPath">
		/// The path to the section in the configuration that contains
		/// the connection string to the MongoDB database.
		/// </param>
		/// <returns>
		/// Returns this instance of the builder for chaining.
		/// </returns>
		public MongoDbWebhookStorageBuilder<TSubscription> Configure(string sectionPath) {
			Services.AddOptions<MongoDbWebhookOptions>()
				.BindConfiguration(sectionPath);

			return this;
		}

		/// <summary>
		/// Configures the MongoDB storage context using the given function
		/// </summary>
		/// <param name="configure">
		/// The configuration function to use to configure the storage context.
		/// </param>
		/// <returns>
		/// Returns the current instance of the builder for chaining.
		/// </returns>
		public MongoDbWebhookStorageBuilder<TSubscription> Configure(Action<MongoDbWebhookOptions> configure) {
			Services.AddOptions<MongoDbWebhookOptions>()
				.Configure(configure);

            return this;
        }

		/// <summary>
		/// Configures the connection string to the MongoDB database
		/// </summary>
		/// <param name="connectionString">
		/// The connection string to the MongoDB database.
		/// </param>
		/// <returns>
		/// Returns the current instance of the builder for chaining.
		/// </returns>
		public MongoDbWebhookStorageBuilder<TSubscription> WithConnectionString(string connectionString) {
			Services.Configure<MongoDbWebhookOptions>(options => options.ConnectionString = connectionString);

			return this;
		}

		/// <summary>
		/// Configures the storage system to use the connection
		/// string with the given name from the application configuration.
		/// </summary>
		/// <param name="connectionStringName">
		/// The name of the connection string to use from the application configuration.
		/// </param>
		/// <returns>
		/// Returns the current instance of the builder for chaining.
		/// </returns>
		public MongoDbWebhookStorageBuilder<TSubscription> WithConnectionStringName(string connectionStringName) {
			Services.AddOptions<MongoDbWebhookOptions>()
                .Configure<IConfiguration>((options, config) => options.ConnectionString = config.GetConnectionString(connectionStringName));

			return this;
		}

		/// <summary>
		/// Changes the scope of the storage to use multi-tenant
		/// infrastructure for connecting to the MongoDB databases
		/// of each tenant.
		/// </summary>
		/// <typeparam name="TTenantInfo">
		/// The type of tenant information resolved, that is holding
		/// the connection string to the MongoDB database of each tenant.
		/// </typeparam>
		/// <returns>
		/// Returns the current instance of the builder for chaining.
		/// </returns>
		public MongoDbWebhookStorageBuilder<TSubscription> UseMultiTenant<TTenantInfo>() where TTenantInfo : class, ITenantInfo, new() {
			Services.RemoveAll<IMongoDbWebhookContext>();
			Services.AddSingleton<IMongoDbWebhookContext, MongoDbWebhookTenantContext<TTenantInfo>>();
			Services.AddSingleton<MongoDbWebhookTenantContext<TTenantInfo>>();

            Services.TryAddSingleton<IWebhookSubscriptionStoreProvider<MongoWebhookSubscription>, MongoDbWebhookSubscriptionStoreProvider<TTenantInfo>>();
            Services.AddSingleton<MongoDbWebhookSubscriptionStoreProvider<TTenantInfo>>();
            Services.TryAddSingleton<MongoDbWebhookSubscriptionStoreProvider<TTenantInfo, MongoWebhookSubscription>>();

            Services.TryAddSingleton<IWebhookDeliveryResultStoreProvider<MongoWebhookDeliveryResult>, MongoDbWebhookDeliveryResultStoreProvider<TTenantInfo>>();
            Services.AddSingleton<MongoDbWebhookDeliveryResultStoreProvider<TTenantInfo>>();
            Services.TryAddSingleton<MongoDbWebhookDeliveryResultStoreProvider<TTenantInfo, MongoWebhookDeliveryResult>>();

            return this;
		}

		/// <summary>
		/// Changes the scope of the storage to use multi-tenant
		/// infrastructure for connecting to the MongoDB databases
		/// of each tenant.
		/// </summary>
		/// <returns></returns>
		public MongoDbWebhookStorageBuilder<TSubscription> UseMultiTenant()
			=> UseMultiTenant<TenantInfo>();

		/// <summary>
		/// Registers the given type of storage to be used for
		/// storing the webhook subscriptions.
		/// </summary>
		/// <typeparam name="TStore">
		/// The type of the storage to use for storing the webhook subscriptions,
		/// that is derived from <see cref="MongoDbWebhookSubscriptionStrore"/>.
		/// </typeparam>
		/// <returns>
		/// Returns the current instance of the builder for chaining.
		/// </returns>
		public MongoDbWebhookStorageBuilder<TSubscription> UseSubscriptionStore<TStore>()
			where TStore : MongoDbWebhookSubscriptionStrore {
			Services.AddSingleton<IWebhookSubscriptionStore<MongoWebhookSubscription>, TStore>();
			Services.AddSingleton<TStore>();

			return this;
		}

		/// <summary>
		/// Registers the given type of storage to be used for
		/// storing the webhook delivery results.
		/// </summary>
		/// <typeparam name="TStore">
		/// The type of the storage to use for storing the webhook delivery results,
		/// derived from <see cref="MongoDbWebhookDeliveryResultStore"/>.
		/// </typeparam>
		/// <returns>
		/// Returns the current instance of the builder for chaining.
		/// </returns>
		public MongoDbWebhookStorageBuilder<TSubscription> UseDeliveryResultStore<TStore>()
			where TStore : MongoDbWebhookDeliveryResultStore {
			Services.AddSingleton<IWebhookDeliveryResultStore<MongoWebhookDeliveryResult>, TStore>();

			return this;
		}

		/// <summary>
		/// Registers an implementation of <see cref="IWebhookDeliveryResultLogger{TWebhook}"/>
		/// that is using MongoDB as the storage for the webhook delivery results.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of the webhook that is being delivered.
		/// </typeparam>
		/// <typeparam name="TResult">
		/// The type of the webhook delivery result that is being logged.
		/// </typeparam>
		/// <returns>
		/// Returns the current instance of the builder for chaining.
		/// </returns>
		public MongoDbWebhookStorageBuilder<TSubscription> UseDeliveryResultLogger<TWebhook, TResult>()
			where TWebhook : class
			where TResult : MongoWebhookDeliveryResult, new() {

			Services.AddSingleton<IWebhookDeliveryResultLogger<TWebhook>, MongoDbWebhookDeliveryResultLogger<TWebhook, TResult>>();

			return this;
		}

		/// <summary>
		/// Registers an implementation of <see cref="IWebhookDeliveryResultLogger{TWebhook}"/>
		/// that is using MongoDB as the storage for the webhook delivery results.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of the webhook that is being delivered.
		/// </typeparam>
		/// <returns>
		/// Returns the current instance of the builder for chaining.
		/// </returns>
		public MongoDbWebhookStorageBuilder<TSubscription> UseDeliveryResultLogger<TWebhook>()
			where TWebhook : class
			=> UseDeliveryResultLogger<TWebhook, MongoWebhookDeliveryResult>();

		/// <summary>
		/// Registers a service that is used to convert the webhook
		/// of the given type to a <see cref="MongoWebhook"/> object
		/// that can be stored by a logger.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of the webhook that is being delivered and that
		/// can be converted to a <see cref="MongoWebhook"/> object.
		/// </typeparam>
		/// <typeparam name="TConverter">
		/// The type of the service that is used to convert the webhook
		/// </typeparam>
		/// <returns>
		/// Returns the current instance of the builder for chaining.
		/// </returns>
		public MongoDbWebhookStorageBuilder<TSubscription> UseWebhookConverter<TWebhook, TConverter>()
			where TWebhook : class
			where TConverter : class, IMongoWebhookConverter<TWebhook> {

            Services.AddSingleton<IMongoWebhookConverter<TWebhook>, TConverter>();

            return this;
        }
	}
}
