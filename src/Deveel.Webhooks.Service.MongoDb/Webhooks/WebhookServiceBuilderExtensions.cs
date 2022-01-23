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
		public static IWebhookServiceBuilder UseMongoDb(this IWebhookServiceBuilder builder, Action<IMongoDbWebhookStorageBuilder> configure = null) {
			var storageBuilder = new MongoDbWebhookStorageBuilderImpl(builder);
			if (configure != null) {
				configure(storageBuilder);
			}

			return builder;
		}

		public static IWebhookServiceBuilder UseMongoDb(this IWebhookServiceBuilder builder, string sectionName, string connectionStringName = null)
			=> builder.UseMongoDb(mongo => mongo.Configure(sectionName, connectionStringName));


		public static IWebhookServiceBuilder UseMongoDb(this IWebhookServiceBuilder builder, Action<MongoDbOptions> configure)
			=> builder.UseMongoDb(mongo => mongo.Configure(configure));

		public static IWebhookServiceBuilder UseMongoDb(this IWebhookServiceBuilder builder, Action<IMongoDbOptionBuilder> configure)
			=> builder.UseMongoDb(mongo => mongo.Configure(configure));
	}
}
