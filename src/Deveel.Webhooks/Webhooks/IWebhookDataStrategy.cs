using System;

namespace Deveel.Webhooks {
	public interface IWebhookDataStrategy {
		IWebhookDataFactory GetDataFactory(EventInfo eventInfo);
	}
}
