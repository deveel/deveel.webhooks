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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Deveel.Webhooks.SendGrid {
	public readonly struct EmailAttachment {
		[JsonConstructor]
		public EmailAttachment(string filename, string type, string content) : this() {
			Filename = filename;
			Type = type;
			Content = content;
		}

		/// <summary>
		/// The base64 encoded content of the attachment.
		/// </summary>
		[JsonPropertyName("content")]
		public string Content { get; }

		/// <summary>
		/// The MIME type of the attachment.
		/// </summary>
		[JsonPropertyName("type")]
		public string Type { get; }

		/// <summary>
		/// The file name of the attachment.
		/// </summary>
		[JsonPropertyName("filename")]
		public string Filename { get; }
	}
}
