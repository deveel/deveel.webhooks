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

using Deveel.Data;

namespace Deveel.Webhooks {
	public static class MongoDbOptionsExtensions {
		public static string DeliveryResultsCollectionName(this MongoDbOptions options)
			=> options.Collections?[MongoDbWebhookStorageConstants.DeliveryResultsCollectionKey];

		public static MongoDbOptions DeliveryResultsCollectionName(this MongoDbOptions options, string collectionName) {
			if (options.Collections == null)
				options.Collections = new Dictionary<string, string>();

			options.Collections[MongoDbWebhookStorageConstants.DeliveryResultsCollectionKey] = collectionName;
			return options;
		}

		public static string SubscriptionsCollectionName(this MongoDbOptions options)
			=> options.Collections?[MongoDbWebhookStorageConstants.SubscriptionCollectionKey];

		public static MongoDbOptions SubscriptionsCollectionName(this MongoDbOptions options, string collectionName) {
			if (options.Collections == null)
				options.Collections = new Dictionary<string, string>();

			options.Collections[MongoDbWebhookStorageConstants.SubscriptionCollectionKey] = collectionName;
			return options;
		}
	}
}
