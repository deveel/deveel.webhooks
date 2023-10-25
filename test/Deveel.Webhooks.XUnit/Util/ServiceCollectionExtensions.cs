// Copyright 2022-2023 Deveel
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using RichardSzalay.MockHttp;

namespace Deveel.Util {
	static class ServiceCollectionExtensions {
		public static IServiceCollection AddTestHttpClient(this IServiceCollection services, IHttpRequestCallback callback) {
			return services.AddSingleton<IHttpClientFactory>(provider => {
				var factory = new MockHttpClientFactory();
				var handler = new MockHttpMessageHandler();
				handler.When("*")
					.Respond(request => callback.RequestsAsync(request, default));

				var client = handler.ToHttpClient();
				factory.AddClient("", client);

				return factory;
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

		class MockHttpClientFactory : IHttpClientFactory {
			public MockHttpClientFactory() {

			}

			public void AddClient(string name, HttpClient client) {
				clients.Add(name, client);
			}

			private readonly Dictionary<string, HttpClient> clients = new Dictionary<string, HttpClient>();

			public HttpClient CreateClient(string name) {
				if (!clients.TryGetValue(name, out var client))
					throw new Exception($"No client with name '{name}' was registered");

				return client;
			}
		}
	}
}
