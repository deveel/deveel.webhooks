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
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	/// <summary>
	/// Represents the result of a validation of a webhook subscription.
	/// </summary>
	public sealed class WebhookValidationResult {
		private WebhookValidationResult(string[]? errors, bool success) {
			Errors = errors;
			Successful = success;
		}

		static WebhookValidationResult() {
			Success = new WebhookValidationResult(null, true);
		}

		/// <summary>
		/// Gets a list of errors that occurred during the validation.
		/// </summary>
		public string[]? Errors { get; }

		/// <summary>
		/// Gets a value indicating whether the validation was successful.
		/// </summary>
		public bool Successful { get; }

		/// <summary>
		/// Gets an instance of the <see cref="WebhookValidationResult"/> that
		/// indicates a successful validation.
		/// </summary>
		public static readonly WebhookValidationResult Success;

		/// <summary>
		/// Creates a new instance of a failed validation.
		/// </summary>
		/// <param name="errors">
		/// A list of errors that occurred during the validation.
		/// </param>
		/// <returns>
		/// Returns a new instance of <see cref="WebhookValidationResult"/>
		/// that represents a failed validation.
		/// </returns>
		public static WebhookValidationResult Failed(params string[] errors)
			=> new WebhookValidationResult(errors, false);
	}
}
