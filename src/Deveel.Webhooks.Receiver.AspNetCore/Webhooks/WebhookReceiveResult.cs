// Copyright 2022-2024 Antonello Provenzano
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
	/// Describes the result of a webhook receive operation.
	/// </summary>
	/// <typeparam name="TWebhook">
	/// The type of webhook that was received
	/// </typeparam>
	/// <remarks>
	/// When this object is returned from a webhook receiver, it can be used
	/// to determine if the webhook was successfully received and if the signature was valid.
	/// </remarks>
	public readonly struct WebhookReceiveResult<TWebhook> where TWebhook : class {
		/// <summary>
		/// Constructs a new result of a webhook receive operation.
		/// </summary>
		/// <param name="webhook">The webhook instance that was received, or <c>null</c>
		/// if it was not possible to receive the webhook for any reason (invalid content,
		/// missing or invalid signature, etc.)</param>
		/// <param name="signatureValid">
		/// Whether the signature of the webhook was valid, or <c>null</c> if the signature
		/// was not checked.
		/// </param>
		public WebhookReceiveResult(TWebhook? webhook, bool? signatureValid = null) {
			Webhooks = webhook == null ? null : new[] {webhook};
			SignatureValid = signatureValid;
		}

		/// <summary>
		/// Constructs a new result of a webhook receive operation.
		/// </summary>
		/// <param name="webhooks"></param>
		/// <param name="signatureValid"></param>
		public WebhookReceiveResult(IList<TWebhook>? webhooks, bool? signatureValid = null) {
			Webhooks = webhooks?.ToList()?.AsReadOnly();
			SignatureValid = signatureValid;
		}

		/// <summary>
		/// Gets the list of webhooks that were received, or <c>null</c> if it was not
		/// possible to receive the webhook for any reason (invalid content, missing or
		/// invalid signature, etc.).
		/// </summary>
		public IReadOnlyList<TWebhook>? Webhooks { get; }

		/// <summary>
		/// Gets the single webhook that was received, or <c>null</c> if it was not
		/// possible to receive the webhook for any reason (invalid content, missing or
		/// invalid signature, etc.), or if more than one webhook was received.
		/// </summary>
		/// <remarks>
		/// It is recommended not to invoke this property, and instead rely on <see cref="Webhooks"/>,
		/// verifying the number of webhooks received, and accessing the list.
		/// </remarks>
		public TWebhook? Webhook => Webhooks?.SingleOrDefault();

		/// <summary>
		/// Gets whether the signature of the webhook was valid, or <c>null</c> if the
		/// signature was not checked.
		/// </summary>
		public bool? SignatureValid { get; }

		/// <summary>
		/// Gets whether the signature of the webhook was validated.
		/// </summary>
		public bool SignatureValidated => SignatureValid.HasValue;

		/// <summary>
		/// Gets whether the webhook was successfully received.
		/// </summary>
		public bool Successful => (Webhooks != null) && (!SignatureValidated || SignatureValid == true);

		/// <summary>
		/// Gets whether the webhook was received but the signature was invalid.
		/// </summary>
		public bool SignatureFailed => SignatureValidated && SignatureValid == false;

		/// <summary>
		/// Creates a new result of a webhook receive operation that indicates that
		/// a signature validation failed.
		/// </summary>
		/// <returns>
		/// Returns a new instance of <see cref="WebhookReceiveResult{TWebhook}"/> that
		/// represents a failed signature validation.
		/// </returns>
		public static WebhookReceiveResult<TWebhook> SignatureFail() => new WebhookReceiveResult<TWebhook>((IList<TWebhook>?) null, false);
	}
}
