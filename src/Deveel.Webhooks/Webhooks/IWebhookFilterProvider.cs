using System;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public interface IWebhookFilterProvider {
		string Name { get; }

		IWebhookFilterEvaluator GetEvaluator();
	}
}
