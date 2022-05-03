﻿// Copyright 2022 Deveel
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
	/// An exception that is thrown during the validation
	/// of a webhook subscription to be created or updated
	/// </summary>
	public class WebhookSubscriptionValidationException : WebhookServiceException {
		public WebhookSubscriptionValidationException(string[] errors = null) : this("The webhook subscription is invalid", errors) {
		}

		public WebhookSubscriptionValidationException(string message, string[] errors = null) : base(message) {
			Errors = errors;
		}

		/// <summary>
		/// Gets a set of errors during the validation of the subscription
		/// </summary>
		public string[] Errors { get; }
	}
}