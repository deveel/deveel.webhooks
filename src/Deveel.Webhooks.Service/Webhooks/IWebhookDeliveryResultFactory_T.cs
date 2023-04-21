using System;
using System.Collections.Generic;
using System.Text;

namespace Deveel.Webhooks {
	public interface IWebhookDeliveryResultFactory<TResult> where TResult : class, IWebhookDeliveryResult {
		TResult CreateResult(IWebhookDeliveryResult deliveryResult);
	}
}
