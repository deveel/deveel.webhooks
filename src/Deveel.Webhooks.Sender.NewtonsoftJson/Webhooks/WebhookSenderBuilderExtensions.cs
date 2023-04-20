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
    /// Extends the <see cref="WebhookSenderBuilder{TWebhook}"/> with
    /// methods to register the <see cref="NewtonsoftWebhookJsonSerializer{TWebhook}"/>
    /// as the default serializer for the webhooks.
    /// </summary>
    public static class WebhookSenderBuilderExtensions {
        /// <summary>
        /// Registers an instance of the <see cref="NewtonsoftWebhookJsonSerializer{TWebhook}"/> 
        /// with the given settings.
        /// </summary>
        /// <typeparam name="TWebhook">
        /// The type of the webhook that is sent by the sender.
        /// </typeparam>
        /// <param name="builder">
        /// The instance of the <see cref="WebhookSenderBuilder{TWebhook}"/> to extend.
        /// </param>
        /// <param name="settings">
        /// An optional set of settings to configure the serializer. When not
        /// provided the default settings are used.
        /// </param>
        /// <returns>
        /// Returns the instance of the builder, to allow chaining.
        /// </returns>
        public static WebhookSenderBuilder<TWebhook> UseNewtonsoftJson<TWebhook>(this WebhookSenderBuilder<TWebhook> builder, JsonSerializerSettings? settings =  null) 
            where TWebhook : class {
            builder.Services.AddSingleton<IWebhookJsonSerializer<TWebhook>>(_ => new NewtonsoftWebhookJsonSerializer<TWebhook>(settings));
            builder.Services.AddSingleton(_ => new NewtonsoftWebhookJsonSerializer<TWebhook>(settings));

            return builder;
        }
    }
}
