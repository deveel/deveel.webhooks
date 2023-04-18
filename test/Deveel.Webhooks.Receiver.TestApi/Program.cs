using Deveel.Webhooks.Handlers;
using Deveel.Webhooks.Model;

using Newtonsoft.Json;

namespace Deveel.Webhooks.Receiver.TestApi {
	public class Program {
		public static void Main(string[] args) {
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddLogging();

			// Add services to the container.
			builder.Services.AddAuthorization();
			builder.Services
				.AddWebhooks<TestWebhook>()
				.UseNewtonsoftJsonParser()
				.AddHandler<TestWebhookHandler>();

			var secret = builder.Configuration["Webhook:Receiver:Signature:Secret"];

			builder.Services.AddWebhooks<TestSignedWebhook>()
				.ConfigureOptions<WebhookReceiverOptions<TestSignedWebhook>>(options => {
					options.VerifySignature = true;
					options.Signature.Secret = secret;
					options.Signature.ParameterName = "X-Webhook-Signature-256";
					options.Signature.Location = WebhookSignatureLocation.Header;
				})
				.UseNewtonsoftJsonParser()
				.UseSigner<Sha256WebhookSigner>()
				.AddHandler<TestSignedWebhookHandler>();

			var app = builder.Build();

			// Configure the HTTP request pipeline.

			app.UseHttpsRedirection();

			app.UseAuthorization();

			app.UseWebhookReceiver<TestWebhook>("/webhook");
			app.UseWebhookReceiver<TestWebhook>("/webhook/handled", (context, webhook) => {
				var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("test");

				logger.LogInformation(JsonConvert.SerializeObject(webhook));
			});

			app.UseWebhookReceiver<TestSignedWebhook>("/webhook/signed");

			app.Run();
		}
	}
}