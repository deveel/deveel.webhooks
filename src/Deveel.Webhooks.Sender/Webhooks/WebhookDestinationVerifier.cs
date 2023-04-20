using System;

using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
    /// <summary>
    /// A default implementation of the <see cref="IWebhookDestinationVerifier"/>,
    /// that pings a destination URL with some configured parameters to verify if it is reachable.
    /// </summary>
    public class WebhookDestinationVerifier : IWebhookDestinationVerifier, IDisposable {
        private HttpClient? httpClient;
        private bool disposeHttpClient;

        private IHttpClientFactory httpClientFactory;
        private bool disposedValue;

        public WebhookDestinationVerifier(IOptions<WebhookDestinationVerifierOptions> options)
            : this(options.Value) {
        }

        protected WebhookDestinationVerifier(WebhookDestinationVerifierOptions verifierOptions) {
            VerifierOptions = verifierOptions;
        }

        ~WebhookDestinationVerifier() {
            Dispose(disposing: false);
        }

        protected WebhookDestinationVerifierOptions VerifierOptions { get; }

        protected virtual HttpClient CreateClient() {
            if (httpClient != null)
                return httpClient;

            if (httpClientFactory != null) {
                return httpClientFactory.CreateClient();
            }

            httpClient = new HttpClient();
            disposeHttpClient = true;

            return httpClient;
        }

        protected virtual HttpRequestMessage CreateRequest(Uri verificationUrl, string token) {
            var request = new HttpRequestMessage(new HttpMethod(VerifierOptions.HttpMethod), verificationUrl);

            if (VerifierOptions.TokenLocation == VerificationTokenLocation.QueryString) {
                var query = System.Web.HttpUtility.ParseQueryString(verificationUrl.Query);
                query[VerifierOptions.TokenQueryParameter] = token;
                var builder = new UriBuilder(verificationUrl) {
                    Query = query.ToString()
                };

                request.RequestUri = builder.Uri;
            } else {
                request.Headers.TryAddWithoutValidation(VerifierOptions.TokenHeaderName, token);
            }

            return request;
        }

        public Task<bool> VerifyDestinationAsync(Uri destinationUrl, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    if (disposeHttpClient)
                        httpClient?.Dispose();
                }

                httpClient = null;
                disposedValue = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
