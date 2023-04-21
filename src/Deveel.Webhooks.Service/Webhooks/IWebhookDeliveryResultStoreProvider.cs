using System;

namespace Deveel.Webhooks {
	public interface IWebhookDeliveryResultStoreProvider<TResult> where TResult : class, IWebhookDeliveryResult {
		IWebhookDeliveryResultStore<TResult> GetTenantStore(string tenantId);
	}
}
