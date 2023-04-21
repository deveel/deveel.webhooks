// Copyright 2022-2023 Deveel
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Webhooks;

using Microsoft.Extensions.Options;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Deveel.Data {
	public abstract class MongoDbStoreBase<TDocument> : IDisposable
		where TDocument : class, IMongoDocument {
		private bool disposed;

		protected MongoDbStoreBase(IOptions<MongoDbOptions> options)
			: this(options.Value) {
		}

		protected MongoDbStoreBase(MongoDbOptions options) {
			Options = options;
		}


		protected abstract IMongoCollection<TDocument> Collection { get; }

		protected MongoDbOptions Options { get; }

		protected IMongoClient Client => CreateClient();

		public bool SupportsPaging => true;

		protected IMongoDatabase Database {
			get {
				var database = Options.DatabaseName;
				if (Options.MultiTenancy != null &&
					Options.MultiTenancy.Handling == MongoDbMultiTenancyHandling.TenantDatabase) {
					database = Options.MultiTenancy.TenantDatabaseFormat
						.Replace("{database}", database)
						.Replace("{tenant}", Options.TenantId);
				}

				return Client.GetDatabase(database);
			}
		}

		protected IMongoCollection<TDocument> GetCollection(string collectionName) {
			if (Options.MultiTenancy != null &&
				Options.MultiTenancy.Handling == MongoDbMultiTenancyHandling.TenantCollection) {
				collectionName = Options.MultiTenancy.TenantCollectionFormat
					.Replace("{collection}", collectionName)
					.Replace("{tenant}", Options.TenantId);
			}

			return Database.GetCollection<TDocument>(collectionName);
		}

		protected FilterDefinition<TDocument> NormalizeFilter(FilterDefinition<TDocument> filter) {
			if (Options.MultiTenancy != null &&
				Options.MultiTenancy.Handling == MongoDbMultiTenancyHandling.TenantField) {
				var tenantFilter = Builders<TDocument>.Filter.Eq(Options.MultiTenancy.TenantField, Options.TenantId);
				filter = Builders<TDocument>.Filter.And(filter, tenantFilter);
			}

			return filter;
		}

		private IMongoClient CreateClient() {
			var settings = MongoClientSettings.FromConnectionString(Options.ConnectionString);

			var pack = new ConventionPack();

			if (Options.CamelCase)
				pack.Add(new CamelCaseElementNameConvention());
			if (Options.EnumAsString)
				pack.Add(new EnumRepresentationConvention(BsonType.String));

			ConventionRegistry.Register("conventions", pack, t => true);

			return new MongoClient(settings);
		}

		protected void ThrowIfDisposed() {
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);
		}

		protected void SetTenantId(TDocument document) {
			if (Options.MultiTenancy != null &&
				Options.MultiTenancy.Handling == MongoDbMultiTenancyHandling.TenantField) {
				var property = typeof(TDocument).GetProperty(Options.MultiTenancy.TenantField);
				if (property != null) {
					property.SetValue(document, Options.TenantId);
				}
			}
		}

		public IQueryable<TDocument> AsQueryable() => Collection.AsQueryable();

		public async Task<string> CreateAsync(TDocument entity, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			var options = new InsertOneOptions();

			SetTenantId(entity);

			await Collection.InsertOneAsync(entity, options, cancellationToken);

			return entity.Id.ToEntityId();
		}

		public async Task<bool> UpdateAsync(TDocument document, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			var filter = Builders<TDocument>.Filter.Eq(x => x.Id, document.Id);
			var result = await Collection.ReplaceOneAsync(filter, document, cancellationToken: cancellationToken);

			return result.IsModifiedCountAvailable && result.ModifiedCount > 0;
		}


		public async Task<TDocument> FindByIdAsync(string id, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, ObjectId.Parse(id));

			filter = NormalizeFilter(filter);

			var result = await Collection.FindAsync(filter, new FindOptions<TDocument, TDocument> { Limit = 1 }, cancellationToken);

			return await result.FirstOrDefaultAsync(cancellationToken);
		}

		public async Task<bool> DeleteAsync(TDocument document, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
			filter = NormalizeFilter(filter);

			var result = await Collection.DeleteOneAsync(filter, cancellationToken: cancellationToken);

			return result.DeletedCount > 0;
		}

		public async Task<int> CountAllAsync(CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			var filter = NormalizeFilter(Builders<TDocument>.Filter.Empty);

			return (int)await Collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
		}

		public async Task<PagedResult<TDocument>> GetPageAsync(PagedQuery<TDocument> query, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			var filter = Builders<TDocument>.Filter.Empty;
			if (query.Predicate != null)
				filter = new ExpressionFilterDefinition<TDocument>(query.Predicate);

			filter = NormalizeFilter(filter);

			var options = new FindOptions<TDocument> {
				Limit = query.PageSize,
				Skip = query.Offset
			};

			var count = await Collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
			var items = await Collection.FindAsync(filter, options, cancellationToken);

			return new PagedResult<TDocument>(query, (int)count, await items.ToListAsync(cancellationToken));
		}

		public virtual void Dispose() {
			disposed = true;
		}
	}
}
