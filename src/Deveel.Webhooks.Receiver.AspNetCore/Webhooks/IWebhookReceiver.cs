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

using Microsoft.AspNetCore.Http;

namespace Deveel.Webhooks {
    /// <summary>
    /// A service that receives a webhook from a remote source.
    /// </summary>
    /// <typeparam name="TWebhook">The type of the webhook that is received</typeparam>
    public interface IWebhookReceiver<TWebhook> where TWebhook : class {
        /// <summary>
        /// Receives a webhook from a remote source, posted through a
        /// HTTP request given.
        /// </summary>
        /// <param name="request">The HTTP request that transports the webhook to be received</param>
        /// <param name="cancellationToken"></param>
        /// <remarks>
        /// <para>
        /// Implementations of this contract should read the content of the request and
        /// parsing it into a webhook instance of the type <typeparamref name="TWebhook"/>.
        /// </para>
        /// <para>
        /// Optionally the implementation may also validate the signature of the request,
        /// to ensure that the webhook is coming from a trusted source: this is not mandatory
        /// but highly recommended. Verification of the signatures of webhook payloads might
        /// affect performances, since the typical implementation of signers use strong hashing
        /// algorithms.
        /// </para>
        /// </remarks>
        /// <returns>
        /// Returns a <see cref="Task"/> that completes when the webhook is received
        /// </returns>
        Task<WebhookReceiveResult<TWebhook>> ReceiveAsync(HttpRequest request, CancellationToken cancellationToken = default);
	}
}
