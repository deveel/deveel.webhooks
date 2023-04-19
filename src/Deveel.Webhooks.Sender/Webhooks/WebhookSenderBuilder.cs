using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Deveel.Webhooks {
	public class WebhookSenderBuilder<TWebhook> where TWebhook : class {
		public IServiceCollection Services { get; }

		public WebhookSenderBuilder(IServiceCollection services) {
			Services = services;

			RegisterDefaultServices();
		}

		private void RegisterDefaultServices() {
			UseSender<WebhookSender<TWebhook>>();
			UseJsonSerializer<SystemTextWebhookJsonSerializer<TWebhook>>();
			UseSigner<Sha256WebhookSigner>();
		}

		public WebhookSenderBuilder<TWebhook> Configure(string sectionPath) {
			Services.AddOptions<WebhookSenderOptions>(typeof(TWebhook).Name)
				.BindConfiguration(sectionPath);

			return this;
		}

		public WebhookSenderBuilder<TWebhook> Configure(Action<WebhookSenderOptions> configure) {
			Services.AddOptions<WebhookSenderOptions>(typeof(TWebhook).Name)
				.Configure(configure);

			return this;
		}

		public WebhookSenderBuilder<TWebhook> UseSender<TSender>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
			where TSender : class, IWebhookSender<TWebhook> {
			Services.TryAdd(new ServiceDescriptor(typeof(IWebhookSender<TWebhook>), typeof(TSender), lifetime));
			Services.TryAdd(new ServiceDescriptor(typeof(TSender), typeof(TSender), lifetime));

			return this;
		}

		public WebhookSenderBuilder<TWebhook> UseJsonSerializer<TSerializer>()
			where TSerializer : class, IWebhookJsonSerializer<TWebhook> {
			Services.TryAddSingleton<IWebhookJsonSerializer<TWebhook>, TSerializer>();

			return this;
		}

		public WebhookSenderBuilder<TWebhook> UseSigner<TSigner>()
			where TSigner : class, IWebhookSigner {

			if (typeof(IWebhookSigner<TWebhook>).IsAssignableFrom(typeof(TSigner))) {
				Services.TryAddSingleton<IWebhookSigner<TWebhook>>(provider => 
					(IWebhookSigner<TWebhook>) provider.GetRequiredService<TSigner>());
			} else {
				Services.TryAddScoped<IWebhookSigner<TWebhook>>(provider => {
					var signer = provider.GetRequiredService<TSigner>();
					return new WebhookSignerAdapter(signer);
				});
			}

			Services.TryAddSingleton<TSigner>();

			return this;
		}

		private class WebhookSignerAdapter : IWebhookSigner<TWebhook> {
			private IWebhookSigner signer;

			public WebhookSignerAdapter(IWebhookSigner signer) {
				this.signer = signer;
			}

			public string[] Algorithms => signer.Algorithms;

			public string SignWebhook(string jsonBody, string secret) => signer.SignWebhook(jsonBody, secret);
		}
	}
}
