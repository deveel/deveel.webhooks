using Microsoft.AspNetCore.Http;

namespace Deveel.Webhooks {
    /// <summary>
    /// A service that can parse a form request into a webhook object.
    /// </summary>
    /// <typeparam name="TWebhook">
    /// The type of webhook object that is parsed from the form request.
    /// </typeparam>
    public interface IWebhookFormParser<TWebhook> where TWebhook : class {
        /// <summary>
        /// Parses the given form request into a webhook object.
        /// </summary>
        /// <param name="form">
        /// The collection of items in the form request.
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// Returns an instance of <typeparamref name="TWebhook"/> that is
        /// the result of parsing the given form request.
        /// </returns>
        Task<TWebhook> ParseWebhookAsync(IFormCollection form, CancellationToken cancellationToken);
    }
}
