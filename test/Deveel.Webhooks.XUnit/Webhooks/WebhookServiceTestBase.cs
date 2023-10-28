﻿// Copyright 2022-2023 Deveel
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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Deveel.Util;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public abstract class WebhookServiceTestBase {
		protected WebhookServiceTestBase(ITestOutputHelper outputHelper) {
			Services = BuildServiceProvider(outputHelper);
		}

		protected IServiceProvider Services { get; }

		private IServiceProvider BuildServiceProvider(ITestOutputHelper outputHelper) {
			return new ServiceCollection()
				.AddWebhookSubscriptions<TestWebhookSubscription>(buidler => ConfigureWebhookService(buidler))
				.AddTestHttpClient(OnRequestAsync)
				.AddLogging(logging => logging.AddXUnit(outputHelper).SetMinimumLevel(LogLevel.Trace))
				.BuildServiceProvider();
		}

		protected virtual Task<HttpResponseMessage> OnRequestAsync(HttpRequestMessage httpRequest) {
			return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
		}

		protected virtual void ConfigureWebhookService(WebhookSubscriptionBuilder<TestWebhookSubscription> builder) {
		}

		protected virtual void ConfigureServices(IServiceCollection services) {

		}
	}
}