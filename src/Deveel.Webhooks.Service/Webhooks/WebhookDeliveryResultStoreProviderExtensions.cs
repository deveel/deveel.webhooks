using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public static class WebhookDeliveryResultStoreProviderExtensions {
		public static Task<TResult> FindByWebhookIdAsync<TResult>(this IWebhookDeliveryResultStoreProvider<TResult> provider, string tenantId, string webhookId, CancellationToken cancellationToken = default)
			where TResult : class, IWebhookDeliveryResult
			=> provider.GetTenantStore(tenantId).FindByWebhookIdAsync(webhookId, cancellationToken);
	}
}
