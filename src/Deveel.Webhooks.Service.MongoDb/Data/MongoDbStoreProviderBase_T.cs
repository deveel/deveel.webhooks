using System;

using Microsoft.Extensions.Options;

namespace Deveel.Data {
	public abstract class MongoDbStoreProviderBase<TDocument> : IStoreProvider<TDocument>
		where TDocument : class, IMongoDocument {
		protected MongoDbStoreProviderBase(IOptions<MongoDbOptions> options)
	: this(options.Value) {
		}

		protected MongoDbStoreProviderBase(MongoDbOptions options) {
			Options = options;
		}

		protected MongoDbOptions Options { get; }

		IStore<TDocument> IStoreProvider<TDocument>.GetTenantStore(string tenantId) => CreateStore(tenantId);

		protected MongoDbStoreBase<TDocument> CreateStore(string tenantId) {
			if (string.IsNullOrWhiteSpace(tenantId))
				throw new ArgumentException($"'{nameof(tenantId)}' cannot be null or whitespace.", nameof(tenantId));

			var storeOptions = new MongoDbOptions {
				TenantId = tenantId,
				ConnectionString = Options.ConnectionString,
				DatabaseName = Options.DatabaseName,
				Collections = Options.Collections,
				MultiTenantHandling = Options.MultiTenantHandling,
				TenantCollectionFormat = Options.TenantCollectionFormat,
				TenantDatabaseFormat = Options.TenantDatabaseFormat,
				TenantField = Options.TenantField,
			};

			return CreateStore(storeOptions);
		}

		protected abstract MongoDbStoreBase<TDocument> CreateStore(MongoDbOptions options);

	}
}