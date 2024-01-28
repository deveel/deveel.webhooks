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
	/// Provides the capabilities to parse a webhook from a JSON stream.
	/// </summary>
	/// <typeparam name="TWebhook">The type of the webhook to be parsed</typeparam>
    public interface IWebhookJsonParser<TWebhook> {
		/// <summary>
		/// Parses a webhook from the given UTF-8 encoded stream.
		/// </summary>
		/// <param name="utf8Stream">The UTF-8 stream that represents the binary
		/// data of a JSON-formatted webhook</param>
		/// <param name="cancellationToken"></param>
		/// <returns>
		/// Returns a <see cref="Task{TResult}"/> that completes when the webhook
		/// stream is parsed and produces the instance of the webhook.
		/// </returns>
		/// <exception cref="WebhookParseException">
		/// Thrown if any error occurs while parsing the webhook stream.
		/// </exception>
		Task<TWebhook?> ParseWebhookAsync(Stream utf8Stream, CancellationToken cancellationToken = default);

		/// <summary>
		/// Parses a list of webhooks from the given UTF-8 encoded stream.
		/// </summary>
		/// <param name="utf8Stream">
		/// The UTF-8 stream that represents the binary data of a JSON-formatted
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a <see cref="Task{TResult}"/> that completes when the webhook
		/// stream is parsed and produces the list of webhooks.
		/// </returns>
		/// <exception cref="WebhookParseException">
		/// Thrown if any error occurs while parsing the webhook stream.
		/// </exception>
		Task<IList<TWebhook>> ParseWebhookArrayAsync(Stream utf8Stream, CancellationToken cancellationToken = default);
	}
}
