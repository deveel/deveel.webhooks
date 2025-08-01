﻿// Copyright 2022-2025 Antonello Provenzano
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

using Deveel.Data;

using MongoFramework;

namespace Deveel.Webhooks {
    /// <summary>
    /// A default instance of the <see cref="IMongoDbWebhookContext"/> that
    /// is used to store the subscriptions and delivery results.
    /// </summary>
    public class MongoDbWebhookContext : MongoDbContext, IMongoDbWebhookContext {
        /// <summary>
        /// Creates an instance of the context with the given options.
        /// </summary>
		/// <param name="connection">
		/// </param>
		public MongoDbWebhookContext(IMongoDbConnection<MongoDbWebhookContext> connection) 
			: base(connection) {
        }

        /// <inheritdoc/>
        protected override void OnConfigureMapping(MappingBuilder mappingBuilder) {
            mappingBuilder.Entity<MongoWebhookSubscription>();
            mappingBuilder.Entity<MongoWebhookDeliveryResult>();

			base.OnConfigureMapping(mappingBuilder);
		}
	}
}
