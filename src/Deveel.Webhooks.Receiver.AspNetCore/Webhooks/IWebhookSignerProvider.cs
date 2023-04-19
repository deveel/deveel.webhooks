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
	/// Implements a provider of <see cref="IWebhookSigner"/> instances
	/// for given algorithms.
	/// </summary>
	/// <typeparam name="TWebhook">The type of webhook to provide signers for</typeparam>
	public interface IWebhookSignerProvider<TWebhook> {
		/// <summary>
		/// Gets the signer for the given algorithm.
		/// </summary>
		/// <param name="algorithm">The name of the algorithm handled by
		/// the signer to lookup for</param>
		/// <returns>
		/// Returns an instance of <see cref="IWebhookSigner"/> that supports
		/// the given algorithm, or <c>null</c> if no such signer is available.
		/// </returns>
		IWebhookSigner? GetSigner(string algorithm);
	}
}
