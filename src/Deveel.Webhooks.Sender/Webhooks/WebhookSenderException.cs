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
	/// An exception thrown when an error occurs while sending a webhook.
	/// </summary>
	public class WebhookSenderException : Exception {
		/// <inheritdoc/>
		public WebhookSenderException() {
		}

        /// <inheritdoc/>
        public WebhookSenderException(string? message) : base(message) {
		}

        /// <inheritdoc/>
        public WebhookSenderException(string? message, Exception? innerException) : base(message, innerException) {
		}

		/// <summary>
		/// Gets the status code of the HTTP response if
		/// the exception is caused by a <see cref="HttpRequestException"/>.
		/// </summary>
		public int? StatusCode => (int?) (InnerException as HttpRequestException)?.StatusCode;
	}
}
