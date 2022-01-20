using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	class DefaultWebhookSenderConfiguration : IWebhookSenderConfiguration {
		private readonly IServiceProvider serviceProvider;

		public DefaultWebhookSenderConfiguration(IOptions<WebhookDeliveryOptions> options, IServiceProvider serviceProvider) {
			DeliveryOptions = options.Value;
			this.serviceProvider = serviceProvider;
		}

		public WebhookDeliveryOptions DeliveryOptions { get; }

		public IWebhookSerializer GetSerializer(string format) {
			var serializerFormat = format;
			if (String.IsNullOrWhiteSpace(serializerFormat))
				serializerFormat = DeliveryOptions.BodyFormat;

			var serializers = serviceProvider.GetServices<IWebhookSerializer>();

			return  serializers.FirstOrDefault(serializer => serializer.Format == serializerFormat);
		}

		public IWebhookSigner GetSigner(string algorithm) {
			var signerAlgorithm = algorithm;
			if (String.IsNullOrWhiteSpace(signerAlgorithm))
				signerAlgorithm = DeliveryOptions.SignatureAlgorithm;

			var signers = serviceProvider.GetServices<IWebhookSigner>();
			return signers?.FirstOrDefault(signer => signer.Algorithm == signerAlgorithm);
		}
	}
}
