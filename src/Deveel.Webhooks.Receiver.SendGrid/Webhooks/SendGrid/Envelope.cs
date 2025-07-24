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

// Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618


using System.Text.Json.Serialization;

namespace Deveel.Webhooks.SendGrid {
	/// <summary>
	/// Describes the envelope of a SendGrid email.
	/// </summary>
	public sealed class Envelope {
		/// <summary>
		/// Gets the list of recipients of the email.
		/// </summary>
		[JsonPropertyName("to")]
		public List<string> To { get; set; }

		/// <summary>
		/// Gets the sender of the email.
		/// </summary>
		[JsonPropertyName("from")]
		public string From { get; set; }

		/// <summary>
		/// Gets or sets the HELO domain of the email.
		/// </summary>
		[JsonPropertyName("helo_domain")]
		public string HeloDomain { get; set; }

		/// <summary>
		/// Gets or sets the IP address of the sender.
		/// </summary>
		[JsonPropertyName("remote_ip")]
		public string RemoteIp { get; set; }

		/// <summary>
		/// Gets or sets the TLS status of the email.
		/// </summary>
		[JsonPropertyName("tls")]
		public bool? Tls { get; set; }

		/// <summary>
		/// Gets or sets the certificate error of the email.
		/// </summary>
		[JsonPropertyName("cert_error")]
		public string CertError { get; set; }
	}
}
