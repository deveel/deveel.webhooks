using System;

using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Webhooks {
	public interface IWebhookConfigurationBuilder {
		IServiceCollection Services { get; }
	}
}
