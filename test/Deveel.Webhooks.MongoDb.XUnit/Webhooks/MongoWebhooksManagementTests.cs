using Bogus;

using MongoDB.Bson;
using MongoDB.Driver;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	[Collection(nameof(MongoWebhookManagementTestCollection))]
	public class MongoWebhooksManagementTests : WebhookManagementTestSuite<MongoWebhookSubscription, ObjectId> {
		private readonly MongoTestDatabase mongo;
		private readonly Faker<MongoWebhookSubscription> faker;

		public MongoWebhooksManagementTests(MongoTestDatabase mongo, ITestOutputHelper testOutput) : base(testOutput) {
			this.mongo = mongo;

			faker = new MongoWebhookSubscriptionFaker();
			MongoClient = new MongoClient(mongo.ConnectionString);
		}

		protected string ConnectionString => mongo.GetConnectionString();

		protected MongoClient MongoClient { get; }

		protected override Faker<MongoWebhookSubscription> Faker => faker;

		protected override ObjectId GenerateSubscriptionKey() => ObjectId.GenerateNewId();

		protected override void ConfigureWebhookStorage(WebhookSubscriptionBuilder<MongoWebhookSubscription, ObjectId> options) {
			options.UseMongoDb(db => db.WithConnectionString(ConnectionString));
		}

		protected override async Task DisposeAsync() {
			await MongoClient.GetDatabase(MongoTestDatabase.DefaultDatabaseName)
				.DropCollectionAsync(MongoDbWebhookStorageConstants.SubscriptionCollectionName);
		}
	}
}
