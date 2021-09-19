using System;

using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Webhooks {
	public interface IWebhookReceiverConfigurationBuilder {
		IWebhookReceiverConfigurationBuilder ConfigureServices(Action<IServiceCollection> configure);
	}
}
