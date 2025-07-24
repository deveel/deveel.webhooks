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

using System.Configuration;

using Deveel.Data;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using MongoDB.Bson;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides a builder to configure the MongoDB storage of webhook entities
	/// </summary>
	/// <typeparam name="TSubscription">
	/// The type of the subscription entity to store in the database.
	/// </typeparam>
	public sealed class MongoDbWebhookStorageBuilder<TSubscription> where TSubscription : MongoWebhookSubscription {
		private readonly WebhookSubscriptionBuilder<TSubscription, ObjectId> builder;

		internal MongoDbWebhookStorageBuilder(WebhookSubscriptionBuilder<TSubscription, ObjectId> builder) {
			this.builder = builder;

			AddDefaultStorage();
		}

		private IServiceCollection Services => builder.Services;

		private void AddDefaultStorage() {
			Services.TryAddScoped<IMongoDbWebhookContext, MongoDbWebhookContext>();

			Services.AddRepository<MongoDbWebhookSubscriptionRepository>();
			Services.AddRepository<MongoDbWebhookDeliveryResultRepository>();

			Services.TryAddSingleton(typeof(IMongoWebhookConverter<>), typeof(DefaultMongoWebhookConverter<>));
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
			Services.AddMongoDbContext<MongoDbWebhookContext>(builder => builder.UseConnection(connectionString));

			return this;
		}

		/// <summary>
		/// Configures the storage system to use the connection
		/// string with the given name from the application configuration.
		/// </summary>
		/// <param name="configuration">
		/// The application configuration that contains the connection strings.
		/// </param>
		/// <param name="connectionStringName">
		/// The name of the connection string to use from the application configuration.
		/// </param>
		/// <returns>
		/// Returns the current instance of the builder for chaining.
		/// </returns>
		public MongoDbWebhookStorageBuilder<TSubscription> WithConnectionStringName(IConfiguration configuration, string connectionStringName) {
			var connectionString = configuration.GetConnectionString(connectionStringName);
			if (string.IsNullOrEmpty(connectionString))
				throw new ConfigurationErrorsException($"No connection string named '{connectionStringName}' was found in the application configuration");

			Services.AddMongoDbContext<MongoDbWebhookContext>(builder => {
				builder.UseConnection(connectionString);
			});

			return this;
		}

		/// <summary>
		/// Registers the given type of storage to be used for
		/// storing the webhook subscriptions.
		/// </summary>
		/// <typeparam name="TRepository">
		/// The type of the storage to use for storing the webhook subscriptions,
		/// that is derived from <see cref="MongoDbWebhookSubscriptionRepository"/>.
		/// </typeparam>
		/// <returns>
		/// Returns the current instance of the builder for chaining.
		/// </returns>
		public MongoDbWebhookStorageBuilder<TSubscription> UseSubscriptionRepository<TRepository>()
			where TRepository : MongoDbWebhookSubscriptionRepository {
			Services.RemoveAll<IRepository<TSubscription>>();
			Services.RemoveAll<IWebhookSubscriptionRepository<TSubscription, ObjectId>>();

			Services.AddRepository<TRepository>();

			return this;
		}

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

		/// <summary>
		/// Registers a function that is used to convert the webhook
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of the webhook that is being delivered and that
		/// should be converted to a <see cref="MongoWebhook"/> object.
		/// </typeparam>
		/// <param name="converter">
		/// The function that is used to convert the webhook
		/// </param>
		/// <returns>
		/// Returns the current instance of the builder for chaining.
		/// </returns>
		public MongoDbWebhookStorageBuilder<TSubscription> UseWebhookConverter<TWebhook>(Func<EventNotification, TWebhook, MongoWebhook> converter)
			where TWebhook : class {

			Services.AddSingleton<IMongoWebhookConverter<TWebhook>>(new MongoWebhookConverterWrapper<TWebhook>(converter));

			return this;
		}

		private class MongoWebhookConverterWrapper<TWebhook> : IMongoWebhookConverter<TWebhook> where TWebhook : class {
			private readonly Func<EventNotification, TWebhook, MongoWebhook> converter;

			public MongoWebhookConverterWrapper(Func<EventNotification, TWebhook, MongoWebhook> converter) {
				this.converter = converter;
			}

			public MongoWebhook ConvertWebhook(EventNotification notification, TWebhook webhook) 
				=> converter.Invoke(notification, webhook);
		}
	}
}
