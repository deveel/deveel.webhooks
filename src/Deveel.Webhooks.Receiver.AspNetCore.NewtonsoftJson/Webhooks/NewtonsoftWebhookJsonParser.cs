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

using System.Text;

using Newtonsoft.Json;

namespace Deveel.Webhooks {
    /// <summary>
    /// Implements a <see cref="IWebhookJsonParser{TWebhook}"/> that uses the
    /// Newtonsoft.Json library to parse the webhook.
    /// </summary>
    /// <typeparam name="TWebhook"></typeparam>
    public sealed class NewtonsoftWebhookJsonParser<TWebhook> : IWebhookJsonParser<TWebhook> {
		/// <summary>
		/// Initializes a new instance of the <see cref="NewtonsoftWebhookJsonParser{TWebhook}"/>
		/// </summary>
		/// <param name="settings">An optional set of configurations that control the
		/// behavior of the JSON serialization. When this is not provided, an instance
		/// of <see cref="Newtonsoft.Json.JsonSerializerSettings"/> is created to provide
		/// the default settings to the serializer.</param>
		public NewtonsoftWebhookJsonParser(JsonSerializerSettings? settings = null) {
			JsonSerializerSettings = settings ?? new JsonSerializerSettings();
		}

		/// <summary>
		/// Gets the settings used to configure the JSON serialization
		/// </summary>
		public JsonSerializerSettings JsonSerializerSettings { get; }

		/// <inheritdoc/>
		public async Task<TWebhook?> ParseWebhookAsync(Stream utf8Stream, CancellationToken cancellationToken = default) {
			try {
				using var textReader = new StreamReader(utf8Stream, Encoding.UTF8);
				var json = await textReader.ReadToEndAsync();

				return JsonConvert.DeserializeObject<TWebhook>(json, JsonSerializerSettings);
			} catch (Exception ex) {
				throw new WebhookParseException("Could not parse the stream to a webhook", ex);
			}
		}

		/// <inheritdoc/>
		public async Task<IList<TWebhook>> ParseWebhookArrayAsync(Stream utf8Stream, CancellationToken cancellationToken = default) {
			try {
				using var textReader = new StreamReader(utf8Stream, Encoding.UTF8);
				var json = await textReader.ReadToEndAsync();

				var result = JsonConvert.DeserializeObject<IList<TWebhook>>(json, JsonSerializerSettings);
				if (result == null)
					result = new List<TWebhook>();

				return result;
			} catch (Exception ex) {
				throw new WebhookParseException("Could not parse the stream to a webhook list", ex);
			}
		}
	}
}
