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
    /// An interface that defines the contract of a service
    /// that is capable of reading XML streams of data and return
    /// a webhook object of type <typeparamref name="TWebhook"/>.
    /// </summary>
    /// <typeparam name="TWebhook">
    /// The type of webhook that is parsed from the XML stream.
    /// </typeparam>
    public interface IWebhookXmlParser<TWebhook> where TWebhook : class {
		/// <summary>
		/// Parses the given <paramref name="utf8Stream"/> and returns
		/// a webhook object of type <typeparamref name="TWebhook"/>.
		/// </summary>
		/// <param name="utf8Stream">
		/// The binary stream of data to parse.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns an instance of <typeparamref name="TWebhook"/> that
		/// is obtained by the reading of the given <paramref name="utf8Stream"/>,
		/// or <c>null</c> if the stream is empty.
		/// </returns>
		Task<TWebhook?> ParseWebhookAsync(Stream utf8Stream, CancellationToken cancellationToken = default);
	}
}
