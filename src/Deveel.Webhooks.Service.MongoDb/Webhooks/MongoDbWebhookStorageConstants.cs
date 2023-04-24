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

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides a set of constants for the MongoDB storage of webhook entities
	/// </summary>
	public static class MongoDbWebhookStorageConstants {
		/// <summary>
		/// The default database name, when not provided in the connection string.
		/// </summary>
		public const string DefaultDatabaseName = "webhooks";

		/// <summary>
		/// The default name of the collection that stores the subscriptions.
		/// </summary>
		public const string SubscriptionCollectionName = "subscriptions";

		/// <summary>
		/// The default name of the collection that stores the delivery results.
		/// </summary>
		public const string DeliveryResultsCollectionName = "delivery_results";
	}
}
