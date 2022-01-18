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

namespace Deveel.Webhooks {
	class MongoDbWebhookOptionBuilder : IMongoDbOptionBuilder {
		public MongoDbWebhookOptions Options { get; }

		public MongoDbWebhookOptionBuilder(MongoDbWebhookOptions options) {
			Options = options;
		}

		public MongoDbWebhookOptionBuilder() {
			Options = new MongoDbWebhookOptions();
		}

		public IMongoDbOptionBuilder SetSubscriptionsCollection(string collectionName) {
			if (string.IsNullOrWhiteSpace(collectionName))
				throw new ArgumentException($"'{nameof(collectionName)}' cannot be null or whitespace.", nameof(collectionName));

			Options.SubscriptionsCollectionName = collectionName;
			return this;
		}

		public IMongoDbOptionBuilder SetConnectionString(string connectionString) {
			if (string.IsNullOrWhiteSpace(connectionString))
				throw new ArgumentException($"'{nameof(connectionString)}' cannot be null or whitespace.", nameof(connectionString));

			Options.ConnectionString = connectionString;
			return this;
		}

		public IMongoDbOptionBuilder SetDatabaseName(string databaseName) {
			if (string.IsNullOrWhiteSpace(databaseName))
				throw new ArgumentException($"'{nameof(databaseName)}' cannot be null or whitespace.", nameof(databaseName));

			Options.DatabaseName = databaseName;
			return this;
		}

		public IMongoDbOptionBuilder SetWebhooksCollection(string collectionName) {
			if (string.IsNullOrWhiteSpace(collectionName))
				throw new ArgumentException($"'{nameof(collectionName)}' cannot be null or whitespace.", nameof(collectionName));

			Options.WebhooksCollectionName = collectionName;
			return this;
		}

		public IMongoDbOptionBuilder SetMultiTenancyHandling(MongoDbMultiTenancyHandling handling) {
			Options.MultiTenantHandling = handling;
			return this;
		}

		public IMongoDbOptionBuilder SetTenantFieldName(string fieldName) {
			if (!String.IsNullOrWhiteSpace(fieldName)) {
				Options.TenantField = fieldName;
				Options.MultiTenantHandling = MongoDbMultiTenancyHandling.TenantField;
			}

			return this;
		}

		public IMongoDbOptionBuilder SetTenantDatabaseFormat(string format) {
			if (!String.IsNullOrWhiteSpace(format)) {
				Options.TenantDatabaseFormat = format;
				Options.MultiTenantHandling = MongoDbMultiTenancyHandling.TenantDatabase;
			}

			return this;
		}

		public IMongoDbOptionBuilder SetTenantCollectionFormat(string format) {
			if (!String.IsNullOrWhiteSpace(format)) {
				Options.TenantCollectionFormat = format;
				Options.MultiTenantHandling = MongoDbMultiTenancyHandling.TenantCollection;
			}

			return this;
		}
	}
}
