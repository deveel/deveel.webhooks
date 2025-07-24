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

using System;

namespace Deveel.Webhooks {
    /// <summary>
    /// Defines a contract for a service that can verify the signature of a
    /// specific type of webhook.
    /// </summary>
    /// <typeparam name="TWebhook">The type of webhook to sign</typeparam>
    /// <remarks>
    /// <para>
    /// Webhook signers are typically implementing the same behavior,
    /// and this contract is a way to define a constraint usage of the signer
    /// within a receiver context.
    /// </para>
    /// <para>
    /// In some advanced scenarios, it is possible to have multiple signers
    /// for the same algorithm but specific for a given type of webhook, according
    /// to the different needs of the provider.
    /// </para>
    /// </remarks>
    /// <see cref="IWebhookSigner"/>
    public interface IWebhookSigner<TWebhook> : IWebhookSigner where TWebhook : class {
	}
}
