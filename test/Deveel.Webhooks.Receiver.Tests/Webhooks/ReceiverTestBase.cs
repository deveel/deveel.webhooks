// Copyright 2022-2024 Antonello Provenzano
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

using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public abstract class ReceiverTestBase<TWebhook> where TWebhook : class {
		protected ReceiverTestBase(ITestOutputHelper outputHelper) {
			OutputHelper = outputHelper;

			var services = new ServiceCollection()
				.AddLogging(logging => logging.AddXUnit(outputHelper, options => {
					options.Filter = (category, level) => true;
				}).SetMinimumLevel(LogLevel.Trace));

			ConfigureServices(services);

			Services = services.BuildServiceProvider();
		}

		protected ITestOutputHelper OutputHelper { get; }

		protected IServiceProvider Services { get; }

		protected IWebhookReceiver<TWebhook> Receiver => Services.GetRequiredService<IWebhookReceiver<TWebhook>>();

		protected virtual void ConfigureServices(IServiceCollection services) {
			AddReceiver(services);
		}

		protected abstract void AddReceiver(IServiceCollection services);

		protected virtual HttpRequest CreateRequest(string path = "/webhook") {
			var httpContext = new DefaultHttpContext();
			httpContext.Request.Method = "POST";
			httpContext.Request.Scheme = "https";
			httpContext.Request.Host = new HostString("localhost");
			httpContext.Request.Path = path;
			httpContext.Request.Body = new MemoryStream();
			httpContext.RequestServices = Services;

			return httpContext.Request;
		}

		protected HttpRequest CreateRequestWithJson(string path, object content) {
			var json = JsonSerializer.Serialize(content, content.GetType());

			return CreateRequestWithJson(path, json);
		}

		protected HttpRequest CreateRequestWithJson(object content)
			=> CreateRequestWithJson("/webhook", content);

		protected virtual HttpRequest CreateRequestWithJson(string path, string json) {
			var request = CreateRequest(path);
			request.Body = new MemoryStream(Encoding.UTF8.GetBytes(json));
			request.ContentType = "application/json";
			return request;
		}

		protected HttpRequest CreateRequestWithJson(string json)
			=> CreateRequestWithJson("/webhook", json);

		protected virtual HttpRequest CreateRequestWithForm(string path, Dictionary<string, StringValues> form) {
			var request = CreateRequest(path);

			request.ContentType = "application/x-www-form-urlencoded";
			request.Form = new FormCollection(form);

			return request;
		}

		protected HttpRequest CreateRequestWithForm(Dictionary<string, StringValues> form)
			=> CreateRequestWithForm("/webhook", form);

		protected virtual async Task<WebhookReceiveResult<TWebhook>> ReceiveAsync(HttpRequest request) {
			return await Receiver.ReceiveAsync(request);
		}
	}
}
