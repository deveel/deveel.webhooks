namespace Deveel.Webhooks {
    /// <summary>
    /// Provides a queryable store for the <see cref="IWebhookDeliveryResultStore{TResult}"/>
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IWebhookDeliveryResultQueryableStore<TResult> : IWebhookDeliveryResultStore<TResult>
        where TResult : class, IWebhookDeliveryResult {
        /// <summary>
        /// Gets a queryable object that can be used to query the store.
        /// </summary>
        /// <returns>
        /// Returns an instance of <see cref="IQueryable{TResult}"/> that can be used
        /// to query the store.
        /// </returns>
        IQueryable<TResult> AsQueryable();
    }
}
