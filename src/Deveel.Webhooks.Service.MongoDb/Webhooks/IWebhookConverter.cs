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

namespace Deveel.Webhooks {
    /// <summary>
    /// A service that is used to convert a webhook to an
    /// object that can be stored in a MongoDB database.
    /// </summary>
    /// <typeparam name="TWebhook">
    /// The type of webhook to convert.
    /// </typeparam>
    public interface IMongoWebhookConverter<TWebhook>
		where TWebhook : class {
        /// <summary>
        /// Converts the given webhook to an object that can be stored
        /// in a MongoDB database.
        /// </summary>
		/// <param name="notification">
		/// The event notification that was sent to the subscribers.
		/// </param>
        /// <param name="webhook">
        /// The instance of the webhook to be converted.
        /// </param>
        /// <returns>
        /// Returns an instance of <see cref="MongoWebhook"/>
        /// that can be stored in a MongoDB database.
        /// </returns>
        MongoWebhook ConvertWebhook(EventNotification notification, TWebhook webhook);
    }
}
