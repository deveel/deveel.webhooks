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
using System.Collections.Generic;

namespace Deveel.Data {
	class MongoDbOptionBuilder : IMongoDbOptionBuilder {
		public MongoDbOptions Options { get; }

		public MongoDbOptionBuilder(MongoDbOptions options) {
			Options = options;
		}

		public MongoDbOptionBuilder() {
			Options = new MongoDbOptions();
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

		public IMongoDbOptionBuilder SetCollectionName(string collectionKey, string collectionName) {
			if (string.IsNullOrWhiteSpace(collectionKey)) 
				throw new ArgumentException($"'{nameof(collectionKey)}' cannot be null or whitespace.", nameof(collectionKey));
			if (string.IsNullOrWhiteSpace(collectionName)) 
				throw new ArgumentException($"'{nameof(collectionName)}' cannot be null or whitespace.", nameof(collectionName));

			if (Options.Collections == null)
				Options.Collections = new Dictionary<string, string>();

			Options.Collections[collectionKey] = collectionName;
			return this;
		}

		public IMongoDbOptionBuilder SetMultiTenancyHandling(MongoDbMultiTenancyHandling handling) {
			if (Options.MultiTenancy == null)
				Options.MultiTenancy = new MongoDbMultiTenantOptions();

			Options.MultiTenancy.Handling = handling;
			return this;
		}

		public IMongoDbOptionBuilder SetTenantFieldName(string fieldName) {
			if (!String.IsNullOrWhiteSpace(fieldName)) {
				if (Options.MultiTenancy == null)
					Options.MultiTenancy = new MongoDbMultiTenantOptions();

				Options.MultiTenancy.TenantField = fieldName;
				Options.MultiTenancy.Handling = MongoDbMultiTenancyHandling.TenantField;
			}

			return this;
		}

		public IMongoDbOptionBuilder SetTenantDatabaseFormat(string format) {
			if (!String.IsNullOrWhiteSpace(format)) {
				if (Options.MultiTenancy == null)
					Options.MultiTenancy = new MongoDbMultiTenantOptions();

				Options.MultiTenancy.TenantDatabaseFormat = format;
				Options.MultiTenancy.Handling = MongoDbMultiTenancyHandling.TenantDatabase;
			}

			return this;
		}

		public IMongoDbOptionBuilder SetTenantCollectionFormat(string format) {
			if (!String.IsNullOrWhiteSpace(format)) {
				if (Options.MultiTenancy == null)
					Options.MultiTenancy = new MongoDbMultiTenantOptions();

				Options.MultiTenancy.TenantCollectionFormat = format;
				Options.MultiTenancy.Handling = MongoDbMultiTenancyHandling.TenantCollection;
			}

			return this;
		}
	}
}
