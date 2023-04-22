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
    /// An implementation of <see cref="IWebhookJsonSerializer{TWebhook}"/>
    /// that uses the <c>Newtonsoft.Json</c> library.
    /// </summary>
    /// <typeparam name="TWebhook">
    /// The type of the webhook to serialize.
    /// </typeparam>
    public sealed class NewtonsoftWebhookJsonSerializer<TWebhook> : IWebhookJsonSerializer<TWebhook>
		where TWebhook : class {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewtonsoftWebhookJsonSerializer{TWebhook}"/>
        /// with the given <paramref name="settings"/>.
        /// </summary>
        /// <param name="settings">
        /// An optional instance of <see cref="JsonSerializerSettings"/> to use for controlling
        /// the serialization. When not provided, a new instance is created to use the defaults.
        /// </param>
        public NewtonsoftWebhookJsonSerializer(JsonSerializerSettings? settings = null) {
            JsonSerializerSettings = settings ?? new JsonSerializerSettings();
        }

        /// <summary>
        /// Gets the settings used for the serialization.
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings { get; }

        /// <inheritdoc/>
        public async Task SerializeAsync(Stream utf8Stream, TWebhook webhook, CancellationToken cancellationToken) {
            try {
                var json = JsonConvert.SerializeObject(webhook, JsonSerializerSettings);

                using var writer = new StreamWriter(utf8Stream, Encoding.UTF8, 1024, true);
                await writer.WriteAsync(new StringBuilder(json), cancellationToken);

                await writer.FlushAsync();

                utf8Stream.Seek(0, SeekOrigin.Begin);
            } catch (Exception ex) {
                throw new WebhookSerializationException("It was not possible to serialize the webhook", ex);
            }
        }
    }
}
