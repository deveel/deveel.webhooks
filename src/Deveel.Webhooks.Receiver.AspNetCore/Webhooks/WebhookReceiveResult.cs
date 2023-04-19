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
		public WebhookReceiveResult(TWebhook? webhook, bool? signatureValid) : this() {
			Webhook = webhook;
			SignatureValid = signatureValid;
		}

		/// <summary>
		/// Gets the webhook instance that was received, or <c>null</c> if it was not
		/// possible to receive the webhook for any reason (invalid content, missing or
		/// invalid signature, etc.).
		/// </summary>
		public TWebhook? Webhook { get; }

		/// <summary>
		/// Gets whether the signature of the webhook was valid, or <c>null</c> if the
		/// signature was not checked.
		/// </summary>
		public bool? SignatureValid { get; }

		/// <summary>
		/// Implicitly converts a <see cref="WebhookReceiveResult{TWebhook}"/> to a
		/// successful result with the given webhook instance.
		/// </summary>
		/// <param name="webhook">
		/// The webhook instance that was received
		/// </param>
		public static implicit operator WebhookReceiveResult<TWebhook>(TWebhook? webhook)
			=> new WebhookReceiveResult<TWebhook>(webhook, null);

		/// <summary>
		/// Gets whether the signature of the webhook was validated.
		/// </summary>
		public bool SignatureValidated => SignatureValid.HasValue;

		/// <summary>
		/// Gets whether the webhook was successfully received.
		/// </summary>
		public bool Successful => Webhook != null && (!SignatureValidated || SignatureValid == true);

		/// <summary>
		/// Gets whether the webhook was received but the signature was invalid.
		/// </summary>
		public bool SignatureFailed => SignatureValidated && SignatureValid == false;
	}
}
