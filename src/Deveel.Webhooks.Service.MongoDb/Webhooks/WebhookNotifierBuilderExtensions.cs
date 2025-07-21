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

using Microsoft.Extensions.DependencyInjection;

using MongoDB.Bson;

namespace Deveel.Webhooks {
    /// <summary>
    /// Provides extensions to the <see cref="WebhookNotifierBuilder{TWebhook}"/>
    /// to register the MongoDB storage for the notifier.
    /// </summary>
    public static class WebhookNotifierBuilderExtensions {
		/// <summary>
		/// Registers the MongoDB storage for resolving
		/// webhook subscriptions to the notifier.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of the webhook to be notified.
		/// </typeparam>
		/// <param name="builder">
		/// The builder of the notifier service where to register
		/// the resolver.
		/// </param>
		/// <returns>
		/// Returns the builder to continue the configuration.
		/// </returns>
		public static WebhookNotifierBuilder<TWebhook> UseMongoSubscriptionResolver<TWebhook>(this WebhookNotifierBuilder<TWebhook> builder)
			where TWebhook : class {
			return builder
				.UseDefaultSubscriptionResolver(typeof(MongoWebhookSubscription));
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
		public static WebhookNotifierBuilder<TWebhook> UseMongoDeliveryResultLogger<TWebhook, TResult>(this WebhookNotifierBuilder<TWebhook> builder)
			where TWebhook : class
			where TResult : MongoWebhookDeliveryResult, new() {

			builder.Services.AddTransient<IWebhookDeliveryResultLogger<TWebhook>, MongoDbWebhookDeliveryResultLogger<TWebhook, TResult>>();

			return builder;
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
		public static WebhookNotifierBuilder<TWebhook> UseMongoDeliveryResultLogger<TWebhook>(this WebhookNotifierBuilder<TWebhook> builder)
			where TWebhook : class
			=> builder.UseMongoDeliveryResultLogger<TWebhook, MongoWebhookDeliveryResult>();
	}
}
