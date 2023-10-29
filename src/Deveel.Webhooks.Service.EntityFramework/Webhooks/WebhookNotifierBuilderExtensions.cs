using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Deveel.Data;

namespace Deveel.Webhooks {
	public static class WebhookNotifierBuilderExtensions {
		public static WebhookNotifierBuilder<TWebhook> UseEntityFrameworkDeliveryResults<TWebhook>(this WebhookNotifierBuilder<TWebhook> builder)
			where TWebhook : class
			=> builder.UseEntityFrameworkDeliveryResults<TWebhook>(typeof(DbWebhookDeliveryResult));


		public static WebhookNotifierBuilder<TWebhook> UseEntityFrameworkDeliveryResults<TWebhook>(this WebhookNotifierBuilder<TWebhook> builder, Type resultType)
			where TWebhook : class {
			ArgumentNullException.ThrowIfNull(resultType, nameof(resultType));

			if (!typeof(DbWebhookDeliveryResult).IsAssignableFrom(resultType))
				throw new ArgumentException($"The given type '{resultType}' is not a valid result type");

			var resultStoreType = typeof(EntityWebhookDeliveryResultRepository<>).MakeGenericType(resultType);
			builder.Services.AddRepository(resultStoreType);

			if (resultType == typeof(DbWebhookDeliveryResult))
				builder.Services.AddRepository<EntityWebhookDeliveryResultRepository>();

			return builder;
		}
	}
}
