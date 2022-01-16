﻿using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using MongoDB.Bson;
using MongoDB.Driver;

namespace Deveel.Webhooks {
	abstract class MongoDbWebhookStoreBase<TDocument, TFacade> : IDisposable 
		where TDocument : class, TFacade, IMongoDocument 
		where TFacade : class {
		private bool disposed;

		protected MongoDbWebhookStoreBase(IOptions<MongoDbWebhookOptions> options)
			: this(options.Value) {
		}

		protected MongoDbWebhookStoreBase(MongoDbWebhookOptions options) {
			Options = options;
		}


		protected abstract IMongoCollection<TDocument> Collection { get; }

		protected MongoDbWebhookOptions Options { get; }

		protected IMongoClient Client => CreateClient();

		protected IMongoDatabase Database => Client.GetDatabase(Options.DatabaseName);

		protected IMongoCollection<TDocument> GetCollection(string collectionName)
			=> Database.GetCollection<TDocument>(collectionName);

		private IMongoClient CreateClient() {
			var settings = MongoClientSettings.FromConnectionString(Options.ConnectionString);
			return new MongoClient(settings);
		}

		protected void ThrowIfDisposed() {
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);
		}

		public Task<string> CreateAsync(TFacade entity, CancellationToken cancellationToken)
			=> CreateAsync((TDocument)entity, cancellationToken);

		public async Task<string> CreateAsync(TDocument entity, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			var options = new InsertOneOptions();

			await Collection.InsertOneAsync(entity, options, cancellationToken);

			return entity.Id.ToEntityId();
		}

		public Task<bool> UpdateAsync(TFacade facade, CancellationToken cancellationToken)
			=> UpdateAsync((TDocument)facade, cancellationToken);

		public async Task<bool> UpdateAsync(TDocument document, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			var filter = Builders<TDocument>.Filter.Eq(x => x.Id, document.Id);
			var result = await Collection.ReplaceOneAsync(filter, document, cancellationToken: cancellationToken);

			return result.IsModifiedCountAvailable && result.ModifiedCount > 0;
		}


		public async Task<TDocument> GetByIdAsync(string id, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, ObjectId.Parse(id));
			var result = await Collection.FindAsync(filter, new FindOptions<TDocument, TDocument> { Limit = 1 }, cancellationToken);

			return await result.FirstOrDefaultAsync(cancellationToken);
		}

		public Task<bool> DeleteAsync(TFacade entity, CancellationToken cancellationToken)
			=> DeleteAsync((TDocument)entity, cancellationToken);

		public async Task<bool> DeleteAsync(TDocument document, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
			var result = await Collection.DeleteOneAsync(filter, cancellationToken: cancellationToken);

			return result.DeletedCount > 0;
		}

		public async Task<int> CountAllAsync(CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			return (int) await Collection.CountDocumentsAsync(Builders<TDocument>.Filter.Empty, cancellationToken: cancellationToken);
		}

		public virtual void Dispose() {
			disposed = true;
		}
	}
}
