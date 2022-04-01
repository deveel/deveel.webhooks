using System;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Webhooks {
	public sealed class WebhookValidationResult {
		private WebhookValidationResult(string[] errors, bool success) {
			Errors = errors;
			Successful = success;
		}

		static WebhookValidationResult() {
			Success = new WebhookValidationResult(null, true);
		}

		public string[] Errors { get; }

		public bool Successful { get; }

		public static readonly WebhookValidationResult Success;

		public static WebhookValidationResult Failed(IEnumerable<string> errors)
			=> new WebhookValidationResult(errors?.ToArray(), false);
	}
}
