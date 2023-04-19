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

using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
    /// <summary>
    /// Extends the <see cref="IOptionsSnapshot{TOptions}"/> interface
    /// to provide standard methods to retrieve the options for a specific
    /// webhook receiver.
    /// </summary>
    public static class OptionsSnapshotExtensions {
        /// <summary>
        /// Gets the options for the webhook receiver of the given type.
        /// </summary>
        /// <typeparam name="TWebhook">The type of webhook handled by the receiver</typeparam>
        /// <param name="options">The instance of the <see cref="IOptionsSnapshot{TOptions}"/> to extend</param>
        /// <returns>
        /// Returns the options for the receiver of the given type.
        /// </returns>
        public static WebhookReceiverOptions GetReceiverOptions<TWebhook>(this IOptionsSnapshot<WebhookReceiverOptions> options)
            => options.Get(typeof(TWebhook).Name);
    }
}
