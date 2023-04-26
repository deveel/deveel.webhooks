using Deveel.Webhooks.Models;
using Deveel.Webhooks.Services;

using Finbuckle.MultiTenant;

namespace Deveel.Webhooks {
	public class Program {
		public static void Main(string[] args) {
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();

			builder.Services.AddUserStore(users => {
				users.AddUser(new User {
					Id = "9911139484",
					Name = "user1",
					Email = "user1@example.com",
					Role = "user"
				});

				users.AddUser(new User {
					Id = "9911139485",
					Name = "admin1",
					Email = "admin@example.com",
					Role = "admin"
				});
			});

			builder.Services.AddMultiTenant<TenantInfo>()
				.WithInMemoryStore(options => {
					options.Tenants.Add(new TenantInfo {
						Id = "339191991",
						Identifier = "tenant1",
						Name = "Tenant 1",
						ConnectionString = builder.Configuration.GetConnectionString("WebhookSubscriptions")
					});
				});

			builder.Services.AddWebhooks<IdentityWebhook>()
				.AddNotifier(notifier => notifier
					.UseWebhookFactory<UserCreatedWebhookFactory>()
					.UseMongoSubscriptionResolver());

			builder.Services.AddWebhookSubscriptions<MongoWebhookSubscription>()
				.UseMongoDb(mongo => mongo.UseMultiTenant());

			var app = builder.Build();

			// Configure the HTTP request pipeline.

			app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}