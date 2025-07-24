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
	/// Configuration options to control the serialization behavior
	/// of XML-formatted webhooks.
	/// </summary>
	public class XmlSerializerOptions {
		/// <summary>
		/// Gets or sets a value indicating XML namespace declarations
		/// in the document should be included (defaults to <c>false</c>).
		/// </summary>
		public bool? IncludeNamespaces { get; set; }

		/// <summary>
		/// Gets or sets a value indicating if the XML declaration should
		/// be included in the document (defaults to <c>false</c>).
		/// </summary>
		public bool? IncludeXmlDeclaration { get; set; }

		/// <summary>
		/// Gets or sets the encoding to use to write the XML document
		/// (defaults to <c>UTF-8</c>).
		/// </summary>
		public string Encoding { get; set; } = "UTF-8";

		/// <summary>
		/// Gets or sets the namespaces to include in the document.
		/// </summary>
		public IDictionary<string, string> Namespaces { get; set; } = new Dictionary<string, string>();

		/// <summary>
		/// Gets or sets a value indicating if the XML document should be
		/// written with indentation (defaults to <c>false</c>).
		/// </summary>
		public bool Indent { get; set; } = false;
	}
}
