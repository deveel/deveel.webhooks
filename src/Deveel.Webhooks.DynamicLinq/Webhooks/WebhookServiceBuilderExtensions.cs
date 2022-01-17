using System;

namespace Deveel.Webhooks {
	public static class WebhookServiceBuilderExtensions {
		public static IWebhookServiceBuilder AddDynamicLinqFilterEvaluator(this IWebhookServiceBuilder builder)
			=> builder.AddFilterEvaluator<LinqWebhookFilterEvaluator>();
	}
}
