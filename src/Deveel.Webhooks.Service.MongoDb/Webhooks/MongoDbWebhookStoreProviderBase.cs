using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	abstract class MongoDbWebhookStoreProviderBase<TDocument, TFacade> 
		where TDocument : class, TFacade, IMongoDocument 
		where TFacade : class {
		protected MongoDbWebhookStoreProviderBase(IOptions<MongoDbWebhookOptions> options)
			: this(options.Value) {
		}

		protected MongoDbWebhookStoreProviderBase(MongoDbWebhookOptions options) {
			Options = options;
		}

		protected MongoDbWebhookOptions Options { get; }

		protected MongoDbWebhookStoreBase<TDocument, TFacade> CreateStore(string tenantId) {
			if (string.IsNullOrWhiteSpace(tenantId)) 
				throw new ArgumentException($"'{nameof(tenantId)}' cannot be null or whitespace.", nameof(tenantId));

			var storeOptions = new MongoDbWebhookOptions {
				TenantId = tenantId,
				ConnectionString = Options.ConnectionString,
				DatabaseName = Options.DatabaseName,
				SubscriptionsCollectionName = Options.SubscriptionsCollectionName,
				WebhooksCollectionName = Options.WebhooksCollectionName,
			};

			return CreateStore(storeOptions);
		}

		protected abstract MongoDbWebhookStoreBase<TDocument, TFacade> CreateStore(MongoDbWebhookOptions options);
	}
}
