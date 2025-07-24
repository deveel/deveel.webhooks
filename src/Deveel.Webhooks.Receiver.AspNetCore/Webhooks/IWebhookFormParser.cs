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
    /// A service that can parse a form request into a webhook object.
    /// </summary>
    /// <typeparam name="TWebhook">
    /// The type of webhook object that is parsed from the form request.
    /// </typeparam>
    public interface IWebhookFormParser<TWebhook> where TWebhook : class {
        /// <summary>
        /// Parses the given form request into a webhook object.
        /// </summary>
        /// <param name="form">
        /// The collection of items in the form request.
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// Returns an instance of <typeparamref name="TWebhook"/> that is
        /// the result of parsing the given form request.
        /// </returns>
        Task<TWebhook> ParseWebhookAsync(IFormCollection form, CancellationToken cancellationToken);
    }
}
