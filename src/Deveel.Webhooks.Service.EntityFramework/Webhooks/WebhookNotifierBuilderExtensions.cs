using Deveel.Data;

namespace Deveel.Webhooks {
	/// <summary>
	/// Extends the <see cref="WebhookNotifierBuilder{TWebhook}"/> to add Entity Framework
	/// support for the delivery results entities.
	/// </summary>
	public static class WebhookNotifierBuilderExtensions {
		/// <summary>
		/// Configures the builder to use the default entity framework delivery 
		/// results support.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of the webhook to be used in the notifier.
		/// </typeparam>
		/// <param name="builder">
		/// The builder to configure.
		/// </param>
		/// <returns>
		/// Returns the same builder instance for chaining calls.
		/// </returns>
		public static WebhookNotifierBuilder<TWebhook> UseEntityFrameworkDeliveryResults<TWebhook>(this WebhookNotifierBuilder<TWebhook> builder)
			where TWebhook : class
			=> builder.UseEntityFrameworkDeliveryResults<TWebhook>(typeof(DbWebhookDeliveryResult));

		/// <summary>
		/// Configures the builder to use the default entity framework delivery 
		/// results support.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of the webhook to be used in the notifier.
		/// </typeparam>
		/// <param name="builder">
		/// The builder to configure.
		/// </param>
		/// <param name="resultType">
		/// The type of the delivery result entity to be used.
		/// </param>
		/// <returns>
		/// Returns the same builder instance for chaining calls.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the given <paramref name="resultType"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown when the given <paramref name="resultType"/> is not a valid
		/// type for the delivery result entity.
		/// </exception>
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
