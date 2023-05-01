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
    /// Provides configurations for the webhooks sender service.
    /// </summary>
    public class WebhookSenderOptions<TWebhook> where TWebhook : class {
		/// <summary>
		/// Gets or sets the name of the HTTP client registered in the
		/// factory pool and that will be used to send the webhooks. 
		/// When this is not provided, the default HTTP client is used.
		/// </summary>
		public string? HttpClientName { get; set; }

		/// <summary>
		/// Gets or sets the default headers to be sent with the webhook,
		/// additionally to the ones specified in the webhook definition.
		/// Any header with the same name of one in the webhook definition
		/// will override the value of the one in the default headers.
		/// </summary>
		public IDictionary<string, string>? DefaultHeaders { get; set; } 
			= new Dictionary<string, string>();

		/// <summary>
		/// Gets or sets the default format to be used when sending a webhook,
		/// used when the destination does not specify a format to be used
		/// (by default set to <see cref="WebhookFormat.Json"/>).
		/// </summary>
		public WebhookFormat DefaultFormat { get; set; } = WebhookSenderDefaults.Format;

		/// <summary>
		/// Gets or sets the options for the retry policy to be used
		/// when sending a webhook fails. Any specification in the
		/// <see cref="WebhookDestination.Retry"/> will override the
		/// configurations provided here.
		/// </summary>
		public WebhookRetryOptions Retry { get; set; } = new WebhookRetryOptions();

		/// <summary>
		/// Gets or sets the timeout for the HTTP request to be sent,
		/// across all retries. When this is not set, the sender has no
		/// timeout to wait for the response.
		/// </summary>
		public TimeSpan? Timeout { get; set; }

		/// <summary>
		/// Gets or sets a flag indicating if the sender should add
		/// trace headers to the HTTP request. When this is not set,
		/// the default value is <c>true</c>.
		/// </summary>
		public bool? AddTraceHeaders { get; set; } = WebhookSenderDefaults.AddTraceHeaders;

		/// <summary>
		/// Gets or sets the name of the header that will be used
		/// to send the session trace ID. When this is not set, the
		/// default value is <c>X-Webhook-TraceId</c>.
		/// </summary>
		public string? TraceHeaderName { get; set; } = WebhookSenderDefaults.TraceHeaderName;

		/// <summary>
		/// Gets or sets the name of the header that will be used to
		/// send the attempt number of the webhook. When this is not
		/// set the default value is <c>X-Webhook-Attempt</c>.
		/// </summary>
		public string? AttemptTraceHeaderName { get; set; } = WebhookSenderDefaults.AttemptTraceHeaderName;

		/// <summary>
		/// Gets or sets the options for the signature of the webhooks. Any
		/// specification in the <see cref="WebhookDestination.Signature"/>
		/// will override the value of these configurations.
		/// </summary>
		public WebhookSenderSignatureOptions Signature { get; set; } 
			= new WebhookSenderSignatureOptions();

		/// <summary>
		/// A service that is used to serialize the webhook to a JSON string.
		/// (by default set to <see cref="SystemTextWebhookJsonSerializer{TWebhook}"/>).
		/// </summary>
		public IWebhookJsonSerializer<TWebhook>? JsonSerializer { get; set; } 
			= new SystemTextWebhookJsonSerializer<TWebhook>();

		/// <summary>
		/// A service that is used to serialize the webhook to an XML string.
		/// (by default set to <see cref="SystemWebhookXmlSerializer{TWebhook}"/>).
		/// </summary>
		public IWebhookXmlSerializer<TWebhook>? XmlSerializer { get; set; } 
			= new SystemWebhookXmlSerializer<TWebhook>();

		/// <summary>
		/// Gets or sets a flag indicating if the webhooks sent should
		/// be signed or not. When this is not set, the sender will
		/// assume that the webhooks should be signed if the signer
		/// is set and the <see cref="Signature"/> configurations are
		/// provided.
		/// </summary>
		public bool? SignWebhooks { get; set; } = WebhookSenderDefaults.SignWebhooks;

		/// <summary>
		/// An instance of a service that is used to sign the webhooks
		/// </summary>
		public IWebhookSigner<TWebhook>? Signer { get; set; }

		/// <summary>
		/// Gets or sets the options for the verification of the receiver
		/// of webhooks that is performed before sending the webhook.
		/// </summary>
		public WebhookReceiverVerificationOptions Verification { get; set; }
			= new WebhookReceiverVerificationOptions();
	}
}