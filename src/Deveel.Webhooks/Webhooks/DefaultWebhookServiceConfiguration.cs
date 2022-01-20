using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	class DefaultWebhookServiceConfiguration : IWebhookServiceConfiguration {
		private readonly IServiceProvider serviceProvider;

		public DefaultWebhookServiceConfiguration(IOptions<WebhookDeliveryOptions> options, IServiceProvider serviceProvider) {
			DeliveryOptions = options.Value;
			this.serviceProvider = serviceProvider;
		}

		public WebhookDeliveryOptions DeliveryOptions { get; }

		public IWebhookFilterEvaluator GetFilterEvaluator(string filterFormat) {
			if (string.IsNullOrWhiteSpace(filterFormat)) 
				throw new ArgumentException($"'{nameof(filterFormat)}' cannot be null or whitespace.", nameof(filterFormat));

			return serviceProvider.GetServices<IWebhookFilterEvaluator>()?
				.FirstOrDefault(evaluator => evaluator.Format == filterFormat);
		}

		public IWebhookSerializer GetSerializer(string format) {
			var serializerFormat = format;
			if (String.IsNullOrWhiteSpace(serializerFormat))
				serializerFormat = DeliveryOptions.BodyFormat;

			return serviceProvider.GetServices<IWebhookSerializer>()?
				.FirstOrDefault(serializer => serializer.Format == serializerFormat);
		}

		public IWebhookSigner GetSigner(string algorithm) {
			var signerAlgorithm = algorithm;
			if (String.IsNullOrWhiteSpace(signerAlgorithm))
				signerAlgorithm = DeliveryOptions.SignatureAlgorithm;

			return serviceProvider.GetServices<IWebhookSigner>()?
				.FirstOrDefault(signer => signer.Algorithm == signerAlgorithm);
		}
	}
}
