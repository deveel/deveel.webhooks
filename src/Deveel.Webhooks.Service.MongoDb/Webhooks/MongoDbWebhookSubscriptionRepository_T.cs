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

using Deveel.Data;

using Microsoft.Extensions.Logging;

using MongoDB.Driver;

using MongoFramework;
using MongoFramework.Linq;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides an implementation of the <see cref="IWebhookSubscriptionRepository{TSubscription}"/>
	/// that is backed by a MongoDB database.
	/// </summary>
	/// <typeparam name="TSubscription">
	/// The type of the webhook subscription, that is
	/// derived from <see cref="MongoWebhookSubscription"/>.
	/// </typeparam>
	public class MongoDbWebhookSubscriptionRepository<TSubscription> : 
		MongoRepository<TSubscription>,
		IWebhookSubscriptionRepository<TSubscription>
		where TSubscription : MongoWebhookSubscription {

		/// <summary>
		/// Constructs the store with the given context.
		/// </summary>
		/// <param name="context">
		/// The context that is used to access the MongoDB database.
		/// </param>
		/// <param name="logger">
		/// The logger instance to use for logging operations.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the given <paramref name="context"/> is <c>null</c>.
		/// </exception>
		public MongoDbWebhookSubscriptionRepository(IMongoDbWebhookContext context, ILogger<MongoDbWebhookSubscriptionRepository<TSubscription>>? logger = null) 
			: base(context, logger) {
		}

		/// <summary>
		/// Gets a set that is used to access the webhook subscriptions
		/// stored in the database.
		/// </summary>
		protected IMongoDbSet<TSubscription> Subscriptions => base.DbSet;

		/// <inheritdoc/>
		public Task<string?> GetDestinationUrlAsync(TSubscription subscription, CancellationToken cancellationToken = default) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult<string?>(subscription.DestinationUrl);
		}

		/// <inheritdoc/>
		public Task SetDestinationUrlAsync(TSubscription subscription, string url, CancellationToken cancellationToken = default) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			subscription.DestinationUrl = url;

			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		public async Task<IList<TSubscription>> GetByEventTypeAsync(string eventType, bool? activeOnly, CancellationToken cancellationToken) {
			try {
				var query = Subscriptions.Where(s => s.EventTypes.Any(y => y == eventType));
				if (activeOnly ?? false)
					query = query.Where(s => s.Status == WebhookSubscriptionStatus.Active);

				return await query.ToListAsync(cancellationToken);
			} catch (Exception ex) {
				throw new WebhookMongoException("Unable to look for subscriptions to events", ex);
			}
		}

		/// <inheritdoc/>
		public Task<string[]> GetEventTypesAsync(TSubscription subscription, CancellationToken cancellationToken = default) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(subscription.EventTypes?.ToArray() ?? Array.Empty<string>());
		}

		/// <inheritdoc/>
		public Task<string?> GetSecretAsync(TSubscription subscription, CancellationToken cancellationToken = default) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(subscription.Secret);
		}

		/// <inheritdoc/>
		public Task SetSecretAsync(TSubscription subscription, string? secret, CancellationToken cancellationToken = default) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			subscription.Secret = secret;

			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		public Task AddEventTypesAsync(TSubscription subscription, string[] eventTypes, CancellationToken cancellationToken = default) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			if (subscription.EventTypes == null)
				subscription.EventTypes = new List<string>();

			subscription.EventTypes.AddRange(eventTypes);

			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		public Task RemoveEventTypesAsync(TSubscription subscription, string[] eventTypes, CancellationToken cancellationToken = default) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			if (subscription.EventTypes == null)
				return Task.CompletedTask;

			foreach (var eventType in eventTypes) {
				subscription.EventTypes.Remove(eventType);
			}

			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		public Task<WebhookSubscriptionStatus> GetStatusAsync(TSubscription subscription, CancellationToken cancellationToken = default) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(subscription.Status);
		}

		/// <inheritdoc/>
		public Task SetStatusAsync(TSubscription subscription, WebhookSubscriptionStatus status, CancellationToken cancellationToken) {
			cancellationToken.ThrowIfCancellationRequested();

			subscription.Status = status;

			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		public Task<IDictionary<string, string>> GetHeadersAsync(TSubscription subscription, CancellationToken cancellationToken = default) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult<IDictionary<string, string>>(subscription.Headers ?? new Dictionary<string, string>());
		}

		/// <inheritdoc/>
		public Task AddHeadersAsync(TSubscription subscription, IDictionary<string, string> headers, CancellationToken cancellationToken = default) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			if (subscription.Headers == null)
				subscription.Headers = new Dictionary<string, string>();

			foreach (var header in headers) {
				subscription.Headers[header.Key] = header.Value;
			}

			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		public Task RemoveHeadersAsync(TSubscription subscription, string[] headerNames, CancellationToken cancellationToken = default) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			if (subscription.Headers == null)
				return Task.CompletedTask;

			foreach (var headerName in headerNames) {
				subscription.Headers.Remove(headerName);
			}

			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		public Task<IDictionary<string, object>> GetPropertiesAsync(TSubscription subscription, CancellationToken cancellationToken = default) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			var props = subscription.Properties ?? new Dictionary<string, object>();
			return Task.FromResult<IDictionary<string, object>>(props);
		}

		/// <inheritdoc/>
		public Task AddPropertiesAsync(TSubscription subscription, IDictionary<string, object> properties, CancellationToken cancellationToken = default) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			if (subscription.Properties == null)
				subscription.Properties = new Dictionary<string, object>();

			foreach (var property in properties) {
				subscription.Properties[property.Key] = property.Value;
			}

			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		public Task RemovePropertiesAsync(TSubscription subscription, string[] propertyNames, CancellationToken cancellationToken = default) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			if (subscription.Properties == null)
				return Task.CompletedTask;

			foreach (var propertyName in propertyNames) {
				subscription.Properties.Remove(propertyName);
			}

			return Task.CompletedTask;
		}
	}
}
