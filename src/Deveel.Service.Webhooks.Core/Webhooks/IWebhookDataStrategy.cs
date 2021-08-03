using System;

namespace Deveel.Webhooks {
	public interface IWebhookDataStrategy {
		IWebhookDataFactory GetDataFactory(string eventType);
	}
}
