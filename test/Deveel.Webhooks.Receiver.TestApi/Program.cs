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
				.AddWebhookReceiver<TestWebhook>()
				.AddHandler<TestWebhookHandler>();

			var secret = builder.Configuration["Webhook:Receiver:Signature:Secret"];

			builder.Services.AddWebhookReceiver<TestSignedWebhook>()
				.Configure(options => {
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

			app.UseWebhookReceiver<TestWebhook>("/webhook");
            app.UseWebhookReceiver<TestWebhook>("/webhook/seq", new WebhookHandlingOptions {
				ExecutionMode = HandlerExecutionMode.Sequential
			});

            app.UseWebhookReceiver("/webhook/handled", (TestWebhook webhook, IWebhookCallback<TestWebhook> callback) => {
				callback.OnWebhookHandled(webhook);
			});

			app.UseWebhookReceiver("/webhook/handled/async", async (TestWebhook webhook, IWebhookCallback<TestWebhook> callback) => {
				await Task.CompletedTask;

				callback.OnWebhookHandled(webhook);
			});

			app.UseWebhookVerfier<TestSignedWebhook>("/webhook/signed");
			app.UseWebhookReceiver<TestSignedWebhook>("/webhook/signed");

			app.Run();
		}
	}
}