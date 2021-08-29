using System;

namespace Deveel.Webhooks {
	public class WebhookFilter : IWebhookFilter {
		public const string Wildcard = "*";

		public WebhookFilter(string expression, string format = null) {
			if (string.IsNullOrWhiteSpace(expression))
				throw new ArgumentException($"'{nameof(expression)}' cannot be null or whitespace.", nameof(expression));

			Expression = expression;
			Format = format;
		}

		public string Expression { get; }

		public string Format { get; set; }

		public static WebhookFilter WildcardFilter => new WebhookFilter(Wildcard);
	}
}
