using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	public static class ServiceRegistrationTests {
		[Fact]
		public static void AddSenderWithDefaults() {
			var services = new ServiceCollection();

			services.AddWebhookSender<Webhook>();

			var provider = services.BuildServiceProvider();

			Assert.NotNull(provider.GetService<IOptions<WebhookSenderOptions<Webhook>>>());
			Assert.NotNull(provider.GetService<IWebhookSender<Webhook>>());
			Assert.NotNull(provider.GetService<WebhookSender<Webhook>>());
		}

		[Fact]
		public static void AddSenderWithOptions() {
			var services = new ServiceCollection();

			services.AddWebhookSender<Webhook>(options => {
				options.Retry.MaxRetries = 3;
				options.DefaultFormat = WebhookFormat.Xml;
			});

			var provider = services.BuildServiceProvider();

			var options = provider.GetRequiredService<IOptions<WebhookSenderOptions<Webhook>>>();

			Assert.NotNull(provider.GetService<IWebhookSender<Webhook>>());
			Assert.NotNull(provider.GetService<WebhookSender<Webhook>>());

			Assert.Equal(3, options.Value.Retry.MaxRetries);
			Assert.Equal(WebhookFormat.Xml, options.Value.DefaultFormat);
		}

		[Fact]
		public static void AddSenderWithConfiguration() {
			var services = new ServiceCollection();

			var configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(new Dictionary<string, string?> {
					{"Webhooks:Sender:Retry:MaxRetries", "3"},
					{"Webhooks:Sender:DefaultFormat", "Xml"}
				})
				.Build();

			services.AddSingleton<IConfiguration>(configuration);

			services.AddWebhookSender<Webhook>("Webhooks:Sender");

			var provider = services.BuildServiceProvider();

			var options = provider.GetRequiredService<IOptions<WebhookSenderOptions<Webhook>>>();

			Assert.NotNull(options);

			Assert.NotNull(provider.GetService<IWebhookSender<Webhook>>());
			Assert.NotNull(provider.GetService<WebhookSender<Webhook>>());

			Assert.Equal(3, options.Value.Retry.MaxRetries);
			Assert.Equal(WebhookFormat.Xml, options.Value.DefaultFormat);
		}

		[Fact]
		public static void AddSenderWithManualOptions() {
			var services = new ServiceCollection();

			services.AddWebhookSender<Webhook>(new WebhookSenderOptions<Webhook> {
				Retry = {
					MaxRetries = 32
				},
				DefaultFormat = WebhookFormat.Xml
			});

			var provider = services.BuildServiceProvider();

			var options = provider.GetRequiredService<IOptions<WebhookSenderOptions<Webhook>>>();

			Assert.NotNull(options);

			Assert.NotNull(provider.GetService<IWebhookSender<Webhook>>());
			Assert.NotNull(provider.GetService<WebhookSender<Webhook>>());

			Assert.Equal(32, options.Value.Retry.MaxRetries);
			Assert.Equal(WebhookFormat.Xml, options.Value.DefaultFormat);
		}
	}

	class Webhook {
	}
}
