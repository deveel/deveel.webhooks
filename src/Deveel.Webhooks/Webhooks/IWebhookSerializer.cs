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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	/// <summary>
	/// The service that is used to serialize a webhook object
	/// in a HTTP request
	/// </summary>
	public interface IWebhookSerializer {
		/// <summary>
		/// Gets the format of the serialized form of the webhook
		/// </summary>
		string Format { get; }


		/// <summary>
		/// Gets a string representation of the webhook
		/// </summary>
		/// <param name="webhook">The webhook payload to be serialized</param>
		/// <param name="cancellationToken"></param>
		/// <returns>
		/// Returns a <see cref="string"/> that represents the webhook in the
		/// format supported by the serialization format.
		/// </returns>
		Task<string> GetAsStringAsync(IWebhook webhook, CancellationToken cancellationToken);

		/// <summary>
		/// Writes the given webhook in the given HTTP request.
		/// </summary>
		/// <param name="requestMessage">The HTTP request where the webhook is written.</param>
		/// <param name="webhook">The webhook payload object to be written to the request</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task WriteAsync(HttpRequestMessage requestMessage, IWebhook webhook, CancellationToken cancellationToken);
	}
}
