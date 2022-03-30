using System;

using Deveel.Webhooks;

using Microsoft.Extensions.Options;

namespace Deveel.Data {
	public abstract class MongoDbStoreProviderBase<TDocument>
		where TDocument : class, IMongoDocument {
		protected MongoDbStoreProviderBase(IOptions<MongoDbOptions> options)
	: this(options.Value) {
		}

		protected MongoDbStoreProviderBase(MongoDbOptions options) {
			Options = options;
		}

		protected MongoDbOptions Options { get; }

		//IWebhookStore<TDocument> IWebhookStoreProvider<TDocument>.GetTenantStore(string tenantId) => CreateStore(tenantId);

		protected MongoDbStoreBase<TDocument> CreateStore(string tenantId) {
			if (string.IsNullOrWhiteSpace(tenantId))
				throw new ArgumentException($"'{nameof(tenantId)}' cannot be null or whitespace.", nameof(tenantId));

			var storeOptions = new MongoDbOptions {
				TenantId = tenantId,
				ConnectionString = Options.ConnectionString,
				DatabaseName = Options.DatabaseName,
				Collections = Options.Collections,
				MultiTenancy = Options.MultiTenancy
			};

			return CreateStore(storeOptions);
		}

		protected abstract MongoDbStoreBase<TDocument> CreateStore(MongoDbOptions options);

	}
}