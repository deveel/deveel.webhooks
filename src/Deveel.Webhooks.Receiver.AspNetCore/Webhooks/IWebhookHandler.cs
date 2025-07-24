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

namespace Deveel.Webhooks {
    /// <summary>
    /// Provides functions for handling webhooks of a specific type.
    /// </summary>
    /// <typeparam name="TWebhook">The type of the webhook to be handled</typeparam>
    /// <remarks>
    /// <para>
    /// The typical usage scenario of usage of services implementing this interface
    /// is within ASP.NET receiver middlewares that are registered in the pipeline, 
    /// and resolve all compatible handlers to notify a webhook has been received by the
    /// application.
    /// </para>
    /// <para>
    /// It is recommended that the implementation of this interface performs a rapid
    /// handling of the webhook, and then delegates the actual processing to a background
    /// or external process, to avoid blocking the pipeline.
    /// </para>
    /// </remarks>
    public interface IWebhookHandler<TWebhook> where TWebhook : class {
		/// <summary>
		/// Handles the given webhook.
		/// </summary>
		/// <param name="webhook">The instance of the webhook to be handled</param>
		/// <param name="cancellationToken"></param>
		/// <returns>
		/// Returns a <see cref="Task"/> that completes when the webhook has been handled.
		/// </returns>
		Task HandleAsync(TWebhook webhook, CancellationToken cancellationToken = default);
	}
}
