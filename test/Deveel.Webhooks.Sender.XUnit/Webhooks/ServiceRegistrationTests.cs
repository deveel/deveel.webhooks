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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	public static class ServiceRegistrationTests {
		[Fact]
		public static void AddSenderWithDefaults() {
			var services = new ServiceCollection();

			services.AddWebhookSender<Webhook>();

			var provider = services.BuildServiceProvider();

			Assert.NotNull(provider.GetService<IOptions<WebhookSenderOptions<Webhook>>>());
			Assert.NotNull(provider.GetService<IWebhookSender<Webhook>>());
			Assert.NotNull(provider.GetService<WebhookSender<Webhook>>());
		}

		[Fact]
		public static void AddSenderWithOptions() {
			var services = new ServiceCollection();

			services.AddWebhookSender<Webhook>(options => {
				options.Retry.MaxRetries = 3;
				options.DefaultFormat = WebhookFormat.Xml;
			});

			var provider = services.BuildServiceProvider();

			var options = provider.GetRequiredService<IOptions<WebhookSenderOptions<Webhook>>>();

			Assert.NotNull(provider.GetService<IWebhookSender<Webhook>>());
			Assert.NotNull(provider.GetService<WebhookSender<Webhook>>());

			Assert.Equal(3, options.Value.Retry.MaxRetries);
			Assert.Equal(WebhookFormat.Xml, options.Value.DefaultFormat);
		}

		[Fact]
		public static void AddSenderWithConfiguration() {
			var services = new ServiceCollection();

			var configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(new Dictionary<string, string?> {
					{"Webhooks:Sender:Retry:MaxRetries", "3"},
					{"Webhooks:Sender:DefaultFormat", "Xml"}
				})
				.Build();

			services.AddSingleton<IConfiguration>(configuration);

			services.AddWebhookSender<Webhook>("Webhooks:Sender");

			var provider = services.BuildServiceProvider();

			var options = provider.GetRequiredService<IOptions<WebhookSenderOptions<Webhook>>>();

			Assert.NotNull(options);

			Assert.NotNull(provider.GetService<IWebhookSender<Webhook>>());
			Assert.NotNull(provider.GetService<WebhookSender<Webhook>>());

			Assert.Equal(3, options.Value.Retry.MaxRetries);
			Assert.Equal(WebhookFormat.Xml, options.Value.DefaultFormat);
		}

		[Fact]
		public static void AddSenderWithManualOptions() {
			var services = new ServiceCollection();

			services.AddWebhookSender<Webhook>(new WebhookSenderOptions<Webhook> {
				Retry = {
					MaxRetries = 32
				},
				DefaultFormat = WebhookFormat.Xml
			});

			var provider = services.BuildServiceProvider();

			var options = provider.GetRequiredService<IOptions<WebhookSenderOptions<Webhook>>>();

			Assert.NotNull(options);

			Assert.NotNull(provider.GetService<IWebhookSender<Webhook>>());
			Assert.NotNull(provider.GetService<WebhookSender<Webhook>>());

			Assert.Equal(32, options.Value.Retry.MaxRetries);
			Assert.Equal(WebhookFormat.Xml, options.Value.DefaultFormat);
		}
	}

	class Webhook {
	}
}
