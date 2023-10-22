using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bogus;

using Finbuckle.MultiTenant;

using Microsoft.Extensions.DependencyInjection;

using MongoDB.Bson;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	[Collection(nameof(MongoTestCollection))]
	public class MongoTenantWebhookManagementTests : WebhookManagementTestSuite<MongoWebhookSubscription> {
		private readonly MongoTestDatabase mongo;
		private readonly Faker<MongoWebhookSubscription> faker;
		private readonly Faker<MongoWebhookSubscription> otherFaker;

		public MongoTenantWebhookManagementTests(MongoTestDatabase mongo, ITestOutputHelper testOutput) 
			: base(testOutput) {
			this.mongo = mongo;
			faker = new MongoWebhookSubscriptionFaker(TenantId);
			otherFaker = new MongoWebhookSubscriptionFaker(OtherTenantId);

			OtherSubscriptions = otherFaker.Generate(120);
		}

		protected string TenantId { get; } = Guid.NewGuid().ToString();

		protected string OtherTenantId { get; } = Guid.NewGuid().ToString();

		protected override object GenerateSubscriptionKey() => ObjectId.GenerateNewId();

		protected override IReadOnlyList<MongoWebhookSubscription> GenerateSubscriptions(int count) => faker.Generate(count);

		protected IReadOnlyList<MongoWebhookSubscription> OtherSubscriptions { get; }

		protected override void ConfigureServices(IServiceCollection services) {
			services.AddSingleton<ITenantInfo>(_ => new TenantInfo {
				Id = TenantId,
				Identifier = TenantId,
				Name = "Test Tenant",
				ConnectionString = mongo.GetConnectionString("webhooks1")
			});

			base.ConfigureServices(services);
		}

		protected override void ConfigureWebhookStorage(WebhookSubscriptionBuilder<MongoWebhookSubscription> options) {
			options.UseMongoDb(db => db.UseMultiTenant());
		}
	}
}
