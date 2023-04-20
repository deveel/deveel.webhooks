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

namespace Deveel.Webhooks {
    /// <summary>
    /// A service that verifies the destination of a webhook, before
    /// attempting a delivery.
    /// </summary>
    /// <typeparam name="TWebhook">
    /// The type of the webhook that is delivered, used to
    /// segregate the verification service to a given scope.
    /// </typeparam>
    /// <remarks>
    /// Implementations of this interface are scoped to a given type 
    /// of webhook.
    /// </remarks>
    public interface IWebhookDestinationVerifier<TWebhook> : IWebhookDestinationVerifier {
    }
}
