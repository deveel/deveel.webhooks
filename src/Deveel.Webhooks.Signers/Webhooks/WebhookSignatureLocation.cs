// Copyright 2022-2024 Antonello Provenzano
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
	/// Enumerates the possible locations where the signature of a webhook
	/// can be found within a HTTP request object.
	/// </summary>
	public enum WebhookSignatureLocation {
		/// <summary>
		/// The signature is found in the HTTP header of the request.
		/// </summary>
		Header,

		/// <summary>
		/// The signature is found in the query string of the request.
		/// </summary>
		QueryString
	}
}
