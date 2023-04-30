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

using Deveel.Webhooks.Handlers;
using Deveel.Webhooks.Model;

namespace Deveel.Webhooks.Receiver.TestApi {
	public class Program {
		public static void Main(string[] args) {
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddLogging();

			// Add services to the container.
			builder.Services.AddAuthorization();
			builder.Services
				.AddWebhookReceiver<TestWebhook>(_ => { })
				.AddHandler<TestWebhookHandler>();

			var secret = builder.Configuration["Webhook:Receiver:Signature:Secret"];

			builder.Services.AddWebhookReceiver<TestSignedWebhook>(options => {
					options.VerifySignature = true;
					options.Signature.Secret = secret;
					options.Signature.Algorithm = "sha256";
					options.Signature.ParameterName = "X-Webhook-Signature-256";
					options.Signature.Location = WebhookSignatureLocation.Header;
					options.Signature.Signer = new Sha256WebhookSigner();
					options.JsonParser = new NewtonsoftWebhookJsonParser<TestSignedWebhook>();
				})
				.AddHandler<TestSignedWebhookHandler>();

			builder.Services.AddWebhookVerifier<TestSignedWebhook>()
				.Configure(options => {
					options.VerificationToken = builder.Configuration["Webhook:Receiver:VerificationToken"];
					options.VerificationTokenQueryName = "token";
				});

			var app = builder.Build();

            app.UseDeveloperExceptionPage();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

			app.UseAuthorization();

			app.MapWebhook<TestWebhook>("/webhook");
            app.MapWebhook<TestWebhook>("/webhook/seq", new WebhookHandlingOptions {
				ExecutionMode = HandlerExecutionMode.Sequential
			});

            app.MapWebhook("/webhook/handled", (TestWebhook webhook, IWebhookCallback<TestWebhook> callback) => {
				callback.OnWebhookHandled(webhook);
			});

			app.MapWebhook("/webhook/handled/async", async (TestWebhook webhook, IWebhookCallback<TestWebhook> callback) => {
				await Task.CompletedTask;

				callback.OnWebhookHandled(webhook);
			});

			app.MapWebhookVerify<TestSignedWebhook>("/webhook/signed");
			app.MapWebhook<TestSignedWebhook>("/webhook/signed");

			app.Run();
		}
	}
}