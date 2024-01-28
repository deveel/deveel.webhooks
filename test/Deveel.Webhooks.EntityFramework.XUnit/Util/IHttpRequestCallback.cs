namespace Deveel.Util {
    interface IHttpRequestCallback {
		Task<HttpResponseMessage> RequestsAsync(HttpRequestMessage request, CancellationToken cancellationToken);
	}
}
