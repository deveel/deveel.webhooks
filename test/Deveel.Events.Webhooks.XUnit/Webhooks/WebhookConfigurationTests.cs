using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace Deveel.Webhooks {
	public static class WebhookConfigurationTests {
		private static IWebhookServiceConfiguration Configure(Action<IServiceCollection> configure) {
			var services = new ServiceCollection();
			configure(services);

			return services.BuildServiceProvider().GetRequiredService<IWebhookServiceConfiguration>();
		}

		private static IWebhookServiceConfiguration Configure(Action<WebhookServiceBuilder<MongoDbWebhookSubscription>> configure = null)
			=> Configure(services => services.AddWebhooks(configure));

		[Fact]
		public static void DefaultConfiguration() {
			var config = Configure();

			Assert.NotNull(config);
			Assert.NotNull(config.DeliveryOptions);
			Assert.Equal(WebhookConfigurationDefaults.SignWebhooks, config.DeliveryOptions.SignWebhooks);
			Assert.Equal(WebhookConfigurationDefaults.SignatureAlgorithm, config.DeliveryOptions.SignatureAlgorithm);
			Assert.Equal(WebhookConfigurationDefaults.SignatureLocation, config.DeliveryOptions.SignatureLocation);
			Assert.NotEmpty(config.Serializers);
			Assert.NotEmpty(config.Signers);
			Assert.Empty(config.DataFactories);
			Assert.Empty(config.FilterEvaluators);
		}

		[Fact]
		public static void DefaultConfigurationWithOtherSettings() {
			var config = Configure(webhooks => webhooks.ConfigureDefaultDelivery().UseMongoDb());

			Assert.NotNull(config);
			Assert.NotNull(config.DeliveryOptions);
			Assert.Equal(WebhookConfigurationDefaults.SignWebhooks, config.DeliveryOptions.SignWebhooks);
			Assert.Equal(WebhookConfigurationDefaults.SignatureAlgorithm, config.DeliveryOptions.SignatureAlgorithm);
			Assert.Equal(WebhookConfigurationDefaults.SignatureLocation, config.DeliveryOptions.SignatureLocation);
			Assert.NotEmpty(config.Serializers);
			Assert.NotEmpty(config.Signers);
			Assert.Empty(config.DataFactories);
			Assert.Empty(config.FilterEvaluators);
		}

		[Fact]
		public static void DeliveryConfigurationBuilder() {
			var config = Configure(webooks => 
				webooks.ConfigureDelivery(delivery => delivery
					.MaxAttempts(4)
					.BodyFormat("xml")
					.SignWebhooks()
					.PlaceSignatureInHeaders()));

			Assert.NotNull(config);
			Assert.NotNull(config.DeliveryOptions);
			Assert.Equal("xml", config.DeliveryOptions.BodyFormat);
			Assert.Equal(4, config.DeliveryOptions.MaxAttemptCount);
			Assert.True(config.DeliveryOptions.SignWebhooks);
			Assert.Equal(WebhookConfigurationDefaults.SignatureAlgorithm, config.DeliveryOptions.SignatureAlgorithm);
			Assert.Equal(WebhookSignatureLocation.Header, config.DeliveryOptions.SignatureLocation);
			Assert.NotEmpty(config.Serializers);
			Assert.NotEmpty(config.Signers);
			Assert.Empty(config.DataFactories);
			Assert.Empty(config.FilterEvaluators);
		}


		[Fact]
		public static void ConfigurationBuilder() {
			var config = Configure(services => {
				var configBuilder = new ConfigurationBuilder()
					.AddInMemoryCollection(new Dictionary<string, string> {
						{ "Webhooks:MaxAttemptCount", "4" },
						{ "Webhooks:BodyFormat", "xml" }
					});

				services.AddSingleton<IConfiguration>(configBuilder.Build())
					.AddWebhooks<MongoDbWebhookSubscription>(webhooks => webhooks.ConfigureDelivery("Webhooks"));
			});

			Assert.NotNull(config);
			Assert.NotNull(config.DeliveryOptions);
			Assert.Equal("xml", config.DeliveryOptions.BodyFormat);
			Assert.Equal(4, config.DeliveryOptions.MaxAttemptCount);
			Assert.Equal(WebhookConfigurationDefaults.SignWebhooks, config.DeliveryOptions.SignWebhooks);
			Assert.Equal(WebhookConfigurationDefaults.SignatureAlgorithm, config.DeliveryOptions.SignatureAlgorithm);
			Assert.Equal(WebhookConfigurationDefaults.SignatureLocation, config.DeliveryOptions.SignatureLocation);
			Assert.NotEmpty(config.Serializers);
			Assert.NotEmpty(config.Signers);
			Assert.Empty(config.DataFactories);
			Assert.Empty(config.FilterEvaluators);
		}
	}
}
