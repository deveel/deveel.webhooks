﻿// Copyright 2022-2024 Antonello Provenzano
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
	/// Provides a set of options for configuring a 
	/// Twilio webhook receiver.
	/// </summary>
	public sealed class TwilioReceiverOptions {
		/// <summary>
		/// Gets or sets the authentication token to use
		/// for the signature verification.
		/// </summary>
		public string? AuthToken { get; set; }

		/// <summary>
		/// Gets or sets the flag indicating if the signature
		/// of the webhook must be verified.
		/// </summary>
		public bool? VerifySignature { get; set; }
	}
}
