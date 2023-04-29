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

using System.Net.Mail;
using System.Text.Json.Serialization;

namespace Deveel.Webhooks.SendGrid {
	public readonly struct EmailAddress {
		[JsonConstructor]
		public EmailAddress(string address, string? name = null) {
			Address = address;
			Name = name;
		}

		[JsonPropertyName("name")]
		public string? Name { get; }

		[JsonPropertyName("email")]
		public string Address { get; }

		/// <summary>
		/// Attempts to parse the given string as an email address.
		/// </summary>
		/// <param name="address">
		/// The string that represents the email address.
		/// </param>
		/// <param name="email">
		/// The parsed email address, if the parsing was successful.
		/// </param>
		/// <returns>
		/// Returns <c>true</c> if the parsing was successful, otherwise <c>false</c>.
		/// </returns>
		public static bool TryParse(string address, out EmailAddress email) {
			if (!MailAddress.TryCreate(address, out var mailAddress)) {
				email = default;
				return false;
			}

			email = new EmailAddress(mailAddress.Address, mailAddress.DisplayName);
			return true;
		}
	}
}
