namespace Deveel.Webhooks {
    /// <summary>
    /// A service that is used to convert a webhook to an
    /// object that can be stored in a MongoDB database.
    /// </summary>
    /// <typeparam name="TWebhook">
    /// The type of webhook to convert.
    /// </typeparam>
    public interface IMongoWebhookConverter<TWebhook> {
        /// <summary>
        /// Converts the given webhook to an object that can be stored
        /// in a MongoDB database.
        /// </summary>
        /// <param name="webhook">
        /// The instance of the webhook to be converted.
        /// </param>
        /// <returns>
        /// Returns an instance of <see cref="MongoWebhook"/>
        /// that can be stored in a MongoDB database.
        /// </returns>
        MongoWebhook ConvertWebhook(TWebhook webhook);
    }
}
