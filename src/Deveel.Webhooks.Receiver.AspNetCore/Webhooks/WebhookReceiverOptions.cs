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

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides the configuration options for a webhook receiver.
	/// </summary>
	public class WebhookReceiverOptions {
		/// <summary>
		/// Gets or sets whether the signature of the incoming webhook
		/// should be verified.
		/// </summary>
		public bool? VerifySignature { get; set; }

		/// <summary>
		/// Gets or sets the options for the signature verification.
		/// </summary>
		public WebhookSignatureOptions? Signature { get; set; } = new WebhookSignatureOptions();

		/// <summary>
		/// Gets or sets the HTTP status code to return when the webhook
		/// processing is successful (<c>201</c> by default).
		/// </summary>
		public int? ResponseStatusCode { get; set; } = 204;

		/// <summary>
		/// Gets or sets the HTTP status code to return when the webhook
		/// processing failed for an internal error (<c>500</c> by default).
		/// </summary>
		public int? ErrorStatusCode { get; set; } = 500;

		/// <summary>
		/// Gets or sets the HTTP status code to return when the webhook
		/// from the sender is invalid (<c>400</c> by default).
		/// </summary>
		public int? InvalidStatusCode { get; set; } = 400;

		/// <summary>
		/// Gets or sets the execution mode for the handlers
		/// during the processing of a received webhook 
		/// (default: <see cref="HandlerExecutionMode.Parallel"/>).
		/// </summary>
		public HandlerExecutionMode? ExecutionMode { get; set; } = HandlerExecutionMode.Parallel;

		/// <summary>
		/// Gets or sets the maximum number of threads to use when
		/// executing the handlers in parallel. By default the number
		/// of processors in the machine is used.
		/// </summary>
		public int? MaxParallelThreads { get; set; }
	}
}
