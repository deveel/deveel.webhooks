using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public interface IWebhookDataFactory {
		bool Handles(EventInfo eventInfo);

		Task<object> CreateDataAsync(EventInfo eventInfo, CancellationToken cancellationToken);
	}
}
