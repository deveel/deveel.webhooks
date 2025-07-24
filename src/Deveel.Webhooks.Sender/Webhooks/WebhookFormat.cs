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
	/// Enumerates the possible formats of
	/// the payloads of a webhook
	/// </summary>
	public enum WebhookFormat {
		/// <summary>
		/// The payload of a webhook is formatted as a JSON object.
		/// </summary>
		Json = 1,

		/// <summary>
		/// The payload of the webhook is formatted as a XML object
		/// </summary>
		Xml = 2,
	}
}
