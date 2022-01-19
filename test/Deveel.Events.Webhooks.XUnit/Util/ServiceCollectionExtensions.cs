using System;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Threading;

using Microsoft.AspNetCore.Http;

using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;

namespace Deveel.Util {
	static class ServiceCollectionExtensions {
		public static IServiceCollection AddTestHttpClient(this IServiceCollection services, IHttpRequestCallback callback) {
			return services.AddSingleton(_ => {
				var handler = new Mock<HttpMessageHandler>();
				handler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
						ItExpr.IsAny<HttpRequestMessage>(),
						ItExpr.IsAny<CancellationToken>())
				.Returns((HttpRequestMessage request, CancellationToken token) => callback.RequestsAsync(request, token));

				return new HttpClient(handler.Object);
			});
		}

		public static IServiceCollection AddTestHttpClient(this IServiceCollection services, Func<HttpRequestMessage, HttpResponseMessage> callback)
			=> services.AddTestHttpClient(new TestHttpRequestCallback(callback));

		public static IServiceCollection AddTestHttpClient(this IServiceCollection services, Func<HttpRequestMessage, Task<HttpResponseMessage>> callback)
			=> services.AddTestHttpClient(new TestHttpRequestAsyncCallback(callback));

		public static IServiceCollection AddTestHttpClient(this IServiceCollection services, Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> callback)
			=> services.AddTestHttpClient(new TestHttpRequestAsyncCallback(callback));


		public static IServiceCollection AddTestHttpClient(this IServiceCollection services)
			=> services.AddTestHttpClient(request => new HttpResponseMessage(HttpStatusCode.OK));
	}
}
