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

// Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618

namespace Deveel.Webhooks.Twilio {
	/// <summary>
	/// Represents a part of a messaging conversation.
	/// </summary>
    public sealed record class MessagePart {
		/// <summary>
		/// Gets the phone number of the messaging part
		/// </summary>
		public string PhoneNumber { get; internal set; }

		/// <summary>
		/// Gets the country code of the messaging part
		/// </summary>
		public string? Country { get; internal set; }

		/// <summary>
		/// Gets the state of the messaging part
		/// </summary>
        public string? State { get; internal set; }

		/// <summary>
		/// Gets the city name of the messaging part
		/// </summary>
        public string? City { get; internal set; }

		/// <summary>
		/// Gets the postal code of the messaging part
		/// </summary>
        public string? Zip { get; internal set; }

		/// <summary>
		/// If the part is a WhatsApp number, gets the phone number
		/// </summary>
		public string WhatsAppPhoneNumber => IsWhatsApp() ? PhoneNumber.Substring(9) : PhoneNumber;

		/// <summary>
		/// Gets a value indicating if the messaging part is a WhatsApp
		/// telephone number.
		/// </summary>
		public bool IsWhatsApp() => PhoneNumber?.StartsWith("whatsapp:") ?? false;
    }
}
