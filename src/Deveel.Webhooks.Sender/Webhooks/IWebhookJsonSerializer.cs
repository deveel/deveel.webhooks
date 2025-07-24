﻿// Copyright 2022-2025 Antonello Provenzano
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
    /// Provides a contract for serializing a webhook to a JSON stream.
    /// </summary>
    /// <typeparam name="TWebhook">
    /// The type of the webhook to serialize.
    /// </typeparam>
    public interface IWebhookJsonSerializer<TWebhook> where TWebhook : class {
        /// <summary>
        /// Serializes the given webhook to the given stream.
        /// </summary>
        /// <param name="utf8Stream">
        /// The UTF-8 stream to which the webhook is serialized.
        /// </param>
        /// <param name="webhook">
        /// The instance of the webhook to serialize.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token to cancel the operation.
        /// </param>
        /// <returns>
        /// Returns a <see cref="Task"/> that completes when the serialization is done.
        /// </returns>
		Task SerializeAsync(Stream utf8Stream, TWebhook webhook, CancellationToken cancellationToken);
	}
}
