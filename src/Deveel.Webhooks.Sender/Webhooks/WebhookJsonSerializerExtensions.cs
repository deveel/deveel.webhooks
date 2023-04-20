using System.Text;

namespace Deveel.Webhooks {
    /// <summary>
    /// Extends the <see cref="IWebhookJsonSerializer{TWebhook}"/> interface
    /// to provide methods to serialize webhoosk.
    /// </summary>
    public static class WebhookJsonSerializerExtensions {
        /// <summary>
        /// Serializes the given webhook to a string.
        /// </summary>
        /// <typeparam name="TWebhook">
        /// The type of the webhook to serialize.
        /// </typeparam>
        /// <param name="serializer">
        /// The instance of the serializer to use.
        /// </param>
        /// <param name="webhook">
        /// The instance of the webhook to serialize.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token to cancel the operation.
        /// </param>
        /// <returns>
        /// Returns a JSON-formatted string containing the serialized webhook.
        /// </returns>
        /// <exception cref="WebhookSerializationException">
        /// Thrown if the webhook cannot be serialized.
        /// </exception>
        public static async Task<string> SerializeWebhookToStringAsync<TWebhook>(this IWebhookJsonSerializer<TWebhook> serializer, TWebhook webhook, CancellationToken cancellationToken)
            where TWebhook : class {

            try {
                using var stream = new MemoryStream();
                await serializer.SerializeWebhookAsync(stream, webhook, cancellationToken);
                stream.Position = 0;

                using var reader = new StreamReader(stream, Encoding.UTF8);
                return await reader.ReadToEndAsync();
            } catch (WebhookSerializationException) {
                throw;
            } catch (Exception ex) {
                throw new WebhookSerializationException("Error while serializing the webhook", ex);
            }
        }
    }
}
