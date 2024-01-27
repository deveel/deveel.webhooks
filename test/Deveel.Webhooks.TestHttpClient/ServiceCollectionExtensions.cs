using System.Net;

using Microsoft.Extensions.DependencyInjection;

using RichardSzalay.MockHttp;

namespace Deveel {
	public static class ServiceCollectionExtensions {
		public static IServiceCollection AddTestHttpCallback(this IServiceCollection services, IHttpRequestCallback callback) {
			var handler = new MockHttpMessageHandler();
			handler.When("*")
				.Respond(request => callback.HandleRequestAsync(request, default));

			return services.AddTestHttpMessageHandlerFactory(handler)
				.AddTestHttpClientFacoty(handler);
		}

		public static IServiceCollection AddTestHttpMessageHandlerFactory(this IServiceCollection services, HttpMessageHandler handler) {
			services.AddSingleton<IHttpMessageHandlerFactory>(new TestHttpMessageHandlerFactory(handler));

			return services;
		}

		public static IServiceCollection AddTestHttpClientFacoty(this IServiceCollection services, HttpMessageHandler handler) {
			services.AddSingleton<IHttpClientFactory>(new TestHttpClientFactory(handler));

			return services;
		}

		public static IServiceCollection AddTestHttpCallback(this IServiceCollection services, Func<HttpRequestMessage, HttpResponseMessage> callback)
			=> services.AddTestHttpCallback(new TestHttpRequestCallback(callback));

		public static IServiceCollection AddHttpCallback(this IServiceCollection services, Func<HttpRequestMessage, Task<HttpResponseMessage>> callback)
			=> services.AddTestHttpCallback(new TestHttpRequestAsyncCallback(callback));

		public static IServiceCollection AddHttpCallback(this IServiceCollection services, Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> callback)
			=> services.AddTestHttpCallback(new TestHttpRequestAsyncCallback(callback));


		public static IServiceCollection AddHttpCallback(this IServiceCollection services)
			=> services.AddTestHttpCallback(request => new HttpResponseMessage(HttpStatusCode.OK));


		class TestHttpMessageHandlerFactory : IHttpMessageHandlerFactory {
			private readonly HttpMessageHandler messageHandler;

			public TestHttpMessageHandlerFactory(HttpMessageHandler messageHandler) {
				this.messageHandler = messageHandler;
			}

			public HttpMessageHandler CreateHandler(string name) {
				return messageHandler;
			}
		}

		class TestHttpClientFactory : IHttpClientFactory {
			private readonly HttpMessageHandler messageHandler;

			public TestHttpClientFactory(HttpMessageHandler messageHandler) {
				this.messageHandler = messageHandler;
			}

			public HttpClient CreateClient(string name) {
				return new HttpClient(messageHandler, false);
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

			public Task<HttpResponseMessage> HandleRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
				if (func != null)
					return func(request);

				if (cancellableFunc != null)
					return cancellableFunc(request, cancellationToken);

				throw new InvalidOperationException();
			}
		}

		class TestHttpRequestCallback : IHttpRequestCallback {
			private readonly Func<HttpRequestMessage, HttpResponseMessage> func;

			public TestHttpRequestCallback(Func<HttpRequestMessage, HttpResponseMessage> func) {
				this.func = func;
			}

			public Task<HttpResponseMessage> HandleRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
				var response = func(request);
				return Task.FromResult(response);
			}

		}
	}
}