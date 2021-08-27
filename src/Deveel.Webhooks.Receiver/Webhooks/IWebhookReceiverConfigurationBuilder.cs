using System;

using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Webhooks {
	public interface IWebhookReceiverConfigurationBuilder {
		IServiceCollection Services { get; }
	}
}
