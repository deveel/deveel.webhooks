using System;

namespace Deveel.Webhooks {
	public sealed class WebhookFilterInfo {
		public WebhookFilterInfo(string expression, string format = null) {
			if (string.IsNullOrWhiteSpace(expression)) 
				throw new ArgumentException($"'{nameof(expression)}' cannot be null or whitespace.", nameof(expression));

			Expression = expression;
			Format = format;
		}

		public string Format { get; }

		public string Expression { get; }

		public bool HasFormat => !String.IsNullOrWhiteSpace(Format);

		public bool IsWildcard => String.Equals(Expression, WebhookFilter.Wildcard);
	}
}
