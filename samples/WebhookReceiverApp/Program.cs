using Deveel.Webhooks.Handlers;
using Deveel.Webhooks.Models;

namespace Deveel.Webhooks {
	public class Program {
		public static void Main(string[] args) {
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddAuthorization();

			builder.Services.AddWebhookReceiver<IdentityWebhook>()
				.AddHandler<UserRegisteredHandler>();


			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
				app.UseHttpsRedirection();

			app.UseAuthorization();

			app.UseWebhookReceiver<IdentityWebhook>("/webhooks/identity");
			app.UseWebhookReceiver("/webhooks/identity/handled", (HttpContext context, IdentityWebhook webhook) => {
				var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
				logger.LogInformation("User {UserName} registered", webhook.User?.Name);
			});

			app.Run();
		}
	}
}