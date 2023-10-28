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

namespace Deveel.Webhooks {
	/// <summary>
	/// A default implementation of the <see cref="IWebhookDeliveryResultRepository{T}"/>
	/// that uses the <see cref="MongoWebhookDeliveryResult"/> as the entity
	/// </summary>
    public class MongoDbWebhookDeliveryResultRepository : MongoDbWebhookDeliveryResultRepository<MongoWebhookDeliveryResult> {
		/// <summary>
		/// Constructs the store with the given context.
		/// </summary>
		/// <param name="context">
		/// The context to the MongoDB database.
		/// </param>
		public MongoDbWebhookDeliveryResultRepository(MongoDbWebhookContext context) : base(context) {
		}
	}
}
