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

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides the configuration options for a webhook receiver.
	/// </summary>
	public class WebhookReceiverOptions<TWebhook> where TWebhook : class {
		/// <summary>
		/// Gets or sets whether the signature of the incoming webhook
		/// should be verified.
		/// </summary>
		public bool? VerifySignature { get; set; }

		/// <summary>
		/// Gets or sets the type of the root object of the incoming webhook.
		/// </summary>
		public WebhookRootType? RootType { get; set; } = WebhookRootType.Object;

		/// <summary>
		/// Gets or sets the options for the signature verification.
		/// </summary>
		public WebhookSignatureOptions Signature { get; set; } = new WebhookSignatureOptions();

		/// <summary>
		/// Gets or sets the supported content formats for the incoming
		/// webhooks that are supported by the receiver.
		/// </summary>
		public WebhookContentFormats ContentFormats { get; set; } = WebhookContentFormats.All;

		/// <summary>
		/// Gets or sets the parser to use to parse the incoming webhook
		/// of JSON content-type, used if the <see cref="ContentFormats"/>
		/// supports <see cref="WebhookContentFormats.Json"/>.
		/// </summary>
		public IWebhookJsonParser<TWebhook>? JsonParser { get; set; } = new SystemTextWebhookJsonParser<TWebhook>();

		/// <summary>
		/// Gets or sets the parser to use to parse the incoming webhook
		/// that are of XML content-type, used if the <see cref="ContentFormats"/>
		/// supports <see cref="WebhookContentFormats.Xml"/>.
		/// </summary>
		public IWebhookXmlParser<TWebhook>? XmlParser { get; set; } = new SystemWebhookXmlParser<TWebhook>();

		/// <summary>
		/// Gets or sets the parser to use to parse the incoming webhook
		/// from the form of the request (when requests are of
		/// content type <c>application/x-www-form-urlencoded</c>), used
		/// when the <see cref="ContentFormats"/> supports <see cref="WebhookContentFormats.Form"/>.
		/// </summary>
		public IWebhookFormParser<TWebhook>? FormParser { get; set; }
	}
}