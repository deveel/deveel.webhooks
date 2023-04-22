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

using System.Text.Json;

namespace Deveel.Webhooks {
	/// <summary>
	/// An implementation of <see cref="IWebhookJsonSerializer{TWebhook}"/> that
	/// uses the functions provided by the <c>System.Text.Json</c> library.
	/// </summary>
	/// <typeparam name="TWebhook">
	/// The type of the webhook that is serialized.
	/// </typeparam>
	public sealed class SystemTextWebhookJsonSerializer<TWebhook> : IWebhookJsonSerializer<TWebhook>
		where TWebhook : class {
		
		/// <summary>
		/// Initializes a new instance of the <see cref="SystemTextWebhookJsonSerializer{TWebhook}"/>
		/// with the given options.
		/// </summary>
		/// <param name="options">
		/// The set of options to use for the serialization. When not provided,
		/// a new instance is created to use the system defaults.
		/// </param>
		public SystemTextWebhookJsonSerializer(JsonSerializerOptions? options = null) {
			JsonSerializerOptions = options ?? new JsonSerializerOptions();
		}

		/// <summary>
		/// Gets the options used for the serialization.
		/// </summary>
        public JsonSerializerOptions JsonSerializerOptions { get; }

		/// <inheritdoc/>
        public async Task SerializeAsync(Stream utf8Stream, TWebhook webhook, CancellationToken cancellationToken) {
			try {
				await JsonSerializer.SerializeAsync(utf8Stream, webhook, JsonSerializerOptions, cancellationToken);
			} catch (Exception ex) {
				throw new WebhookSerializationException("It was not possible to serialize the webhook", ex);
			}
		}
	}
}
