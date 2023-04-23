using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Deveel.Util {
	class TestHttpRequestCallback : IHttpRequestCallback {
		private readonly Func<HttpRequestMessage, HttpResponseMessage> func;

		public TestHttpRequestCallback(Func<HttpRequestMessage, HttpResponseMessage> func) {
			this.func = func;
		}

		public Task<HttpResponseMessage> RequestsAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
			var response = func(request);
			return Task.FromResult(response);
		}

	}

	class TestHttpRequestAsyncCallback : IHttpRequestCallback {
		private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>>? func;
		private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>? cancellableFunc;

		public TestHttpRequestAsyncCallback(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> func) {
			cancellableFunc = func;
		}

		public TestHttpRequestAsyncCallback(Func<HttpRequestMessage, Task<HttpResponseMessage>> func) {
			this.func = func;
		}

		public Task<HttpResponseMessage> RequestsAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
			if (func != null)
				return func(request);

			if (cancellableFunc != null)
				return cancellableFunc(request, cancellationToken);

			throw new InvalidOperationException();
		}
	}
}
