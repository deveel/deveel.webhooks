// Copyright 2022 Deveel
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
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides the algorithms to form a signatur 
	/// to be used when delivering a webhook 
	/// </summary>
	public interface IWebhookSigner {
		/// <summary>
		/// Gets the algorithm used to create the signature
		/// </summary>
		string Algorithm { get; }

		/// <summary>
		/// Creates a signature that is used to secure the
		/// delivery of a webhook
		/// </summary>
		/// <param name="payloadJson">The JSON representation of the 
		/// payload of a webhook</param>
		/// <param name="secret">A secret phrase provided by the
		/// subscriber, that is used to compute the signature</param>
		/// <returns>
		/// Returns a secure string that can be used to sign
		/// the webhook against the receiver.
		/// </returns>
		string Sign(string payloadJson, string secret);
	}
}
