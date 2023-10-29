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
	/// An exception that denotes an error during the execution
	/// of the webhook service
	/// </summary>
	public class WebhookServiceException : OperationException {
		/// <inheritdoc/>
		public WebhookServiceException(string errorCode) : base(errorCode) {
		}

		/// <inheritdoc/>
		public WebhookServiceException(string errorCode, string? message) : base(errorCode, message) {
		}

		/// <inheritdoc/>
		public WebhookServiceException(string errorCode, string? message, Exception innerException) : base(errorCode, message, innerException) {
		}
	}
}
