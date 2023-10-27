using Deveel.Data;

using Finbuckle.MultiTenant;

using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	[Collection(nameof(MongoTestCollection))]
	public class MongoDeliveryResultLoggingTests : DeliveryResultLoggerTestSuite {
		private readonly MongoTestDatabase mongo;

		public MongoDeliveryResultLoggingTests(MongoTestDatabase mongo, ITestOutputHelper testOutput) : base(testOutput) {
			this.mongo = mongo;
		}

		private IRepositoryProvider<MongoWebhookDeliveryResult> RepositoryProvider 
			=> Scope!.ServiceProvider.GetRequiredService<IRepositoryProvider<MongoWebhookDeliveryResult>>();

		protected override void ConfigureService(IServiceCollection services) {
			services.AddSingleton<TenantInfo>(_ => {
				return new TenantInfo {
					Id = TenantId,
					Identifier = TenantId
				};
			});

			services.AddMultiTenant<TenantInfo>()
				.WithInMemoryStore(store => {
					store.Tenants.Add(new TenantInfo {
						Id = TenantId,
						Identifier = TenantId,
						Name = "Test Tenant",
						ConnectionString = mongo.GetConnectionString("webhooks1")
					});

					store.Tenants.Add(new TenantInfo {
						Id = "tenant2",
						Identifier = "tenant2",
						Name = "Test Tenant 2",
						ConnectionString = mongo.GetConnectionString("webhooks2")
					});
				});

			services.AddSingleton<IMongoWebhookConverter<Webhook>, DefaultMongoWebhookConverter<Webhook>>();
			services.AddMongoDbContext<MongoDbWebhookTenantContext>((tenant, builder) => builder.UseConnection(tenant.ConnectionString!));
			services.AddRepositoryProvider<MongoDbWebhookDeliveryResultRepositoryProvider<TenantInfo>>();
			services.AddScoped<IWebhookDeliveryResultLogger<Webhook>, MongoDbWebhookDeliveryResultLogger<Webhook, MongoWebhookDeliveryResult>>();
		}

		protected override async Task<IWebhookDeliveryResult?> FindResultByOperationIdAsync(string operationId) {
			var respository = await RepositoryProvider.GetRepositoryAsync(TenantId);
			return await respository.FindFirstAsync(x => x.OperationId == operationId);
		}
	}
}
