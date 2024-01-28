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

using System.Text.Json;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides a default implementation of the <see cref="IWebhookJsonParser{TWebhook}"/>
	/// that is using the <c>System.Text.Json</c> library for parsing the JSON
	/// representations of webhooks.
	/// </summary>
	/// <typeparam name="TWebhook">The type of the webhook to parse</typeparam>
    public sealed class SystemTextWebhookJsonParser<TWebhook> : IWebhookJsonParser<TWebhook> where TWebhook : class {
		/// <summary>
		/// Initializes a new instance of the <see cref="SystemTextWebhookJsonParser{TWebhook}"/>
		/// </summary>
		/// <param name="options">A set of options to control the behavior of the serialization</param>
		/// <remarks>
		/// When the <paramref name="options"/> is not provided, a new instance of the
		/// <see cref="System.Text.Json.JsonSerializerOptions"/> is created with the
		/// default configurations.
		/// </remarks>
		public SystemTextWebhookJsonParser(JsonSerializerOptions? options = null) {
			JsonSerializerOptions = options ?? new JsonSerializerOptions();
		}

		/// <summary>
		/// Gets the options used to control the behavior of the serialization
		/// </summary>
		public JsonSerializerOptions JsonSerializerOptions { get; }

		/// <inheritdoc/>
		public async Task<IList<TWebhook>> ParseWebhookArrayAsync(Stream utf8Stream, CancellationToken cancellationToken = default) {
			try {
				return (await JsonSerializer.DeserializeAsync<IList<TWebhook>>(utf8Stream, JsonSerializerOptions, cancellationToken))!;
			} catch (Exception ex) {
				throw new WebhookParseException("Could not parse the stream to a webhook array", ex);
			}
		}

		/// <inheritdoc/>
		public async Task<TWebhook?> ParseWebhookAsync(Stream utf8Stream, CancellationToken cancellationToken = default) {
			try {
				return await JsonSerializer.DeserializeAsync<TWebhook>(utf8Stream, JsonSerializerOptions, cancellationToken);
			} catch (Exception ex) {
				throw new WebhookParseException("Could not parse the stream to a webhook", ex);
			}
		}
	}
}
