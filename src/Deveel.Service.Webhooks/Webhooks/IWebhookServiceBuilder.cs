using System;

using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Webhooks {
	public interface IWebhookServiceBuilder {
		void ConfigureServices(Action<IServiceCollection> configure);
	}
}
