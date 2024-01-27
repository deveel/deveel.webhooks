namespace Deveel {
	public interface IHttpRequestCallback {
		Task<HttpResponseMessage> HandleRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken);
	}
}
