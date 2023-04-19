
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

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace Deveel.Webhooks {
    /// <summary>
    /// Extends the <see cref="WebhookReceiverBuilder{TWebhook}"/> to registering
    /// the parser for the webhook payload using the Newtonsoft.Json library.
    /// </summary>
    public static class WebhookReceiverBuilderExtensions {
        /// <summary>
        /// Registers the <see cref="NewtonsoftWebhookJsonParser{TWebhook}"/> as
        /// a parser for webhook payloads.
        /// </summary>
        /// <typeparam name="TWebhook">The type of webhook to parse</typeparam>
        /// <param name="builder">The builder object used to configure a webhook receiver</param>
        /// <param name="settings">A set of settings to configure the JSON serialization process</param>
        /// <returns>
        /// Returns the <paramref name="builder"/> instance with the parser registered
        /// </returns>
        public static WebhookReceiverBuilder<TWebhook> UseNewtonsoftJsonParser<TWebhook>(this WebhookReceiverBuilder<TWebhook> builder, JsonSerializerSettings? settings = null)
            where TWebhook : class {

            builder.Services.AddSingleton<IWebhookJsonParser<TWebhook>>(_ => new NewtonsoftWebhookJsonParser<TWebhook>(settings));
            builder.Services.AddSingleton(_ => new NewtonsoftWebhookJsonParser<TWebhook>(settings));


            return builder;
        }
    }
}
