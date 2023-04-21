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
using System.Collections.Generic;

namespace Deveel.Data {
	/// <summary>
	/// The configuration options for the MongoDB data layer
	/// of the webhook management.
	/// </summary>
	public sealed class MongoDbOptions {
		/// <summary>
		/// Gets or sets the name of the database.
		/// </summary>
		public string DatabaseName { get; set; }

		/// <summary>
		/// Gets or sets a dictionary of the names of collections
		/// </summary>
		public IDictionary<string, string> Collections { get; set; }

		/// <summary>
		/// Gets or sets the connection string to the MongoDB server
		/// </summary>
		public string ConnectionString { get; set; }

		/// <summary>
		/// Gets or sets the identifier of the tenant
		/// </summary>
		public string TenantId { get; set; }

		/// <summary>
		/// Gets or sets whether the serialization of the documents
		/// should be camel-case.
		/// </summary>
		public bool CamelCase { get; set; } = true;

		/// <summary>
		/// Gets or sets whether the serialization of enumerations
		/// in documents should be as done strings
		/// </summary>
		public bool EnumAsString { get; set; } = true;

		public MongoDbMultiTenantOptions MultiTenancy { get; set; } = new MongoDbMultiTenantOptions();

		///// <summary>
		///// Gets or sets the type of handling of the multi-tenant scenarios of usage
		///// </summary>
		//public MongoDbMultiTenancyHandling MultiTenantHandling { get; set; } = MongoDbMultiTenancyHandling.TenantField;

		public bool IsCollectionSet(string collectionKey)
			=> Collections?.ContainsKey(collectionKey) ?? false;
	}
}
