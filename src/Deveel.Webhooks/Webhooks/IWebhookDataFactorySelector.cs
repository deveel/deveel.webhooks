using System;
using System.Collections.Generic;
using System.Text;

namespace Deveel.Webhooks {
	public interface IWebhookDataFactorySelector {
		IWebhookDataFactory GetDataFactory(EventInfo eventInfo);
	}
}
