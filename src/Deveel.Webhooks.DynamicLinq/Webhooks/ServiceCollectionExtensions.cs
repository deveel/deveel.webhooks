using System;

using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Webhooks {
	public static class ServiceCollectionExtensions {
		public static IServiceCollection AddDynamicLinqFilterEvaluator(this IServiceCollection services)
			=> services.AddSingleton<IWebhookFilterEvaluator, LinqWebhookFilterEvaluator>();
	}
}
