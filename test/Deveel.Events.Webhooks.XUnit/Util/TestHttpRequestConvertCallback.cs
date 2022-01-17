using System;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Threading;

using Microsoft.AspNetCore.Http;

using Microsoft.Extensions.Primitives;
using System.Linq;

namespace Deveel.Util {
	class TestHttpRequestConvertCallback : IHttpRequestCallback {
		private Func<HttpContext, CancellationToken, Task> asyncCallback;
		private Action<HttpContext> syncCallback;

		public TestHttpRequestConvertCallback(Action<HttpContext> syncCallback) {
			this.syncCallback = syncCallback;
		}

		public TestHttpRequestConvertCallback(Func<HttpContext, CancellationToken, Task> callback) {
			this.asyncCallback = callback;
		}

		public async Task<HttpResponseMessage> RequestsAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
			var context = new DefaultHttpContext();

			foreach (var header in request.Headers) {
				context.Request.Headers.Add(header.Key, new StringValues(header.Value.ToArray()));
			}

			context.Request.ContentType = request.Content.Headers.ContentType.ToString();
			context.Request.ContentLength = request.Content.Headers.ContentLength;

			var requestStream = new MemoryStream();
			await request.Content.CopyToAsync(requestStream);

			requestStream.Position = 0;

			context.Request.Method = request.Method.ToString();
			if (request.RequestUri != null) {
				context.Request.Host = new HostString(request.RequestUri.Host);
				context.Request.QueryString = new QueryString(request.RequestUri.Query);
				context.Request.Scheme = request.RequestUri.Scheme;
				context.Request.Path = request.RequestUri.AbsolutePath;
			}

			context.Request.Body = requestStream;

			if (asyncCallback != null) {
				await asyncCallback(context, cancellationToken);
			} else if (syncCallback != null) {
				syncCallback(context);
			}

			var response = new HttpResponseMessage((HttpStatusCode)context.Response.StatusCode);
			response.RequestMessage = request;

			var responseBody = new MemoryStream();
			await context.Response.Body.CopyToAsync(responseBody);

			response.Content = new StreamContent(responseBody);

			foreach (var header in context.Response.Headers) {
				response.Headers.Add(header.Key, header.Value.ToList());
			}

			return response;
		}
	}
}
