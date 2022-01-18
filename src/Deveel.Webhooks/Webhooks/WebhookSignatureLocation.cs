﻿// Copyright 2022 Deveel
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
	/// Enumerates the possible locations of a webhook
	/// signature within an HTTP request.
	/// </summary>
	public enum WebhookSignatureLocation {
		/// <summary>
		/// The signature is provided in the header of the request
		/// </summary>
		Header,

		/// <summary>
		/// The signature is provided as an parameter in the
		/// query string
		/// </summary>
		QueryString
	}
}
