using System;

namespace Deveel.Webhooks {
	public class WebhookFilter : IWebhookFilter {
		public const string Wildcard = "*";

		public WebhookFilter(string provider, string expression, string format = null) {
			if (string.IsNullOrWhiteSpace(provider)) 
				throw new ArgumentException($"'{nameof(provider)}' cannot be null or whitespace.", nameof(provider));
			if (string.IsNullOrWhiteSpace(expression))
				throw new ArgumentException($"'{nameof(expression)}' cannot be null or whitespace.", nameof(expression));

			Provider = provider;
			Expression = expression;
			ExpressionFormat = format;
		}

		public string Expression { get; }

		public string Provider { get; }

		public string ExpressionFormat { get; set; }

		public static WebhookFilter Default(string expression, string format = null)
			=> new WebhookFilter("default", expression, format);

		public static WebhookFilter DefaultWildcard => Default(Wildcard);
	}
}
