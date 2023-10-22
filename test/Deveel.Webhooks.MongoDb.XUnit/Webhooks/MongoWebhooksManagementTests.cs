using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bogus;

using MongoDB.Bson;
using MongoDB.Driver;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	[Collection(nameof(MongoTestCollection))]
	public class MongoWebhooksManagementTests : WebhookManagementTestSuite<MongoWebhookSubscription> {
		private readonly MongoTestDatabase mongo;
		private readonly Faker<MongoWebhookSubscription> faker;

		public MongoWebhooksManagementTests(MongoTestDatabase mongo, ITestOutputHelper testOutput) : base(testOutput) {
			this.mongo = mongo;

			faker = new MongoWebhookSubscriptionFaker();
			MongoClient = new MongoClient(mongo.ConnectionString);
		}

		protected string ConnectionString => mongo.GetConnectionString();

		protected MongoClient MongoClient { get; }

		protected override object GenerateSubscriptionKey() => ObjectId.GenerateNewId();

		protected override IReadOnlyList<MongoWebhookSubscription> GenerateSubscriptions(int count) => faker.Generate(count);

		protected override void ConfigureWebhookStorage(WebhookSubscriptionBuilder<MongoWebhookSubscription> options) {
			options.UseMongoDb(db => db.WithConnectionString(ConnectionString));
		}
	}
}
