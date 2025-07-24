// Copyright 2022-2025 Antonello Provenzano
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
	/// A service that converts a <see cref="IWebhook"/> object into a <see cref="DbWebhook"/>
	/// that can be stored in the database.
	/// </summary>
	/// <typeparam name="TWebhook">
	/// The type of the webhook object to be converted.
	/// </typeparam>
    public interface IDbWebhookConverter<TWebhook> where TWebhook : class {
		/// <summary>
		/// Converts the given <paramref name="webhook"/> into a <see cref="DbWebhook"/>
		/// </summary>
		/// <param name="eventInfo">
		/// The <see cref="EventInfo"/> that describes the event that triggered the webhook.
		/// </param>
		/// <param name="webhook">
		/// The webhook object to be converted.
		/// </param>
		/// <returns>
		/// Returns an instance of <see cref="DbWebhook"/> that can be stored in the database.
		/// </returns>
        DbWebhook ConvertWebhook(EventInfo eventInfo, TWebhook webhook);
    }
}
