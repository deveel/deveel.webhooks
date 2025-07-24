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
	/// Provides a contract for serializing a webhook to a XML stream.
	/// </summary>
	/// <typeparam name="TWebhook"></typeparam>
	public interface IWebhookXmlSerializer<TWebhook> where TWebhook : class {
		/// <summary>
		/// Serialized a webhook as XML to the given stream.
		/// </summary>
		/// <param name="utf8Stream">
		/// A UTF-8 encoded stream that is the destination
		/// of the serialized webhook.
		/// </param>
		/// <param name="webhook">
		/// The instance of the webhook to serialize.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns an asynchronous operation that when completed
		/// </returns>
		Task SerializeAsync(Stream utf8Stream, TWebhook webhook, CancellationToken cancellationToken = default);
	}
}
