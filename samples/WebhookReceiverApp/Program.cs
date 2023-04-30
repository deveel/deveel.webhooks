using Deveel.Webhooks.Handlers;
using Deveel.Webhooks.Models;

namespace Deveel.Webhooks {
	public class Program {
		public static void Main(string[] args) {
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddAuthorization();

			builder.Services.AddWebhookReceiver<IdentityWebhook>(new WebhookReceiverOptions<IdentityWebhook>())
				.AddHandler<UserRegisteredHandler>();


			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
				app.UseHttpsRedirection();

			app.UseAuthorization();

			app.MapWebhook<IdentityWebhook>("/webhooks/identity");
			app.MapWebhook("/webhooks/identity/handled", (IdentityWebhook webhook, ILogger<Program> logger) => {
				logger.LogInformation("User {UserName} registered", webhook.User?.Name);
			});

			app.Run();
		}
	}
}