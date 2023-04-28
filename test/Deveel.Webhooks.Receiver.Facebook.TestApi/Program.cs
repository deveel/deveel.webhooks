using Deveel.Webhooks.Facebook;

namespace Deveel.Webhooks {
    public class Program {
		public static void Main(string[] args) {
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddAuthorization();

			builder.Services.AddFacebookReceiver("Facebook");

			var app = builder.Build();

			// Configure the HTTP request pipeline.

			app.UseHttpsRedirection();

			app.UseAuthorization();

			app.MapFacebookWebhook("/webhooks/facebook", (FacebookWebhook webhook, IWebhookCallback<FacebookWebhook> callback) => {
				callback.OnWebhookHandled(webhook);
			})
				.MapFacebookVerify();

			app.Run();
		}
	}
}