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

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Deveel.Webhooks {
	/// <summary>
	/// An implementation of <see cref="IWebhookSubscriptionRepository{TSubscription}"/> that
	/// uses an <see cref="DbContext"/> to store the subscriptions.
	/// </summary>
	/// <typeparam name="TSubscription">
	/// The type of the subscription entity to be stored.
	/// </typeparam>
	/// <seealso cref="IWebhookSubscriptionRepository{TSubscription}"/>"/>
	public class EntityWebhookSubscriptionRepository<TSubscription> :
		EntityRepository<TSubscription>,
		IWebhookSubscriptionRepository<TSubscription>
		where TSubscription : DbWebhookSubscription {

		/// <summary>
		/// Constructs the store by using the given <see cref="WebhookDbContext"/>.
		/// </summary>
		/// <param name="context">
		/// The database context to be used to store the subscriptions.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the given <paramref name="context"/> is <c>null</c>.
		/// </exception>
		public EntityWebhookSubscriptionRepository(WebhookDbContext context, ILogger<EntityWebhookSubscriptionRepository<TSubscription>>? logger = null)
			: base(context, logger) {
		}

		public override IQueryable<TSubscription> AsQueryable() {
			return Context.Set<TSubscription>()
				.Include(x => x.Filters)
				.Include(x => x.Headers)
				.Include(x => x.Events)
				.Include(x => x.Properties);
		}

		private async Task<TSubscription> EnsureLoadedAsync(TSubscription subscription, CancellationToken cancellationToken) {
			var entry = Context.Entry(subscription);
			if (!entry.Collection(x => x.Headers).IsLoaded)
				await entry.Collection(x => x.Headers).LoadAsync(cancellationToken);
			if (!entry.Collection(x => x.Events).IsLoaded)
				await entry.Collection(x => x.Events).LoadAsync(cancellationToken);
			if (!entry.Collection(x => x.Filters).IsLoaded)
				await entry.Collection(x => x.Filters).LoadAsync(cancellationToken);
			if (!entry.Collection(x => x.Properties).IsLoaded)
				await entry.Collection(x => x.Properties).LoadAsync(cancellationToken);

			return subscription;
		}

		/// <inheritdoc/>
		protected override async Task<TSubscription> OnEntityFoundByKeyAsync(object key, TSubscription entity, CancellationToken cancellationToken = default)
			=> await EnsureLoadedAsync(entity, cancellationToken);

		/// <inheritdoc/>
		public Task<string> GetDestinationUrlAsync(TSubscription subscription, CancellationToken cancellationToken = default) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(subscription.DestinationUrl);
		}

		/// <inheritdoc/>
		public Task SetDestinationUrlAsync(TSubscription subscription, string destinationUrl, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			subscription.DestinationUrl = destinationUrl;

			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		public async Task<IList<TSubscription>> GetByEventTypeAsync(string eventType, bool? activeOnly, CancellationToken cancellationToken = default) {
			try {
				var query = new QueryBuilder<TSubscription>()
					.Where(x => x.Events.Any(y => y.EventType == eventType));

				if (activeOnly ?? false)
					query = query.Where(x => x.Status == WebhookSubscriptionStatus.Active);

				return await base.FindAllAsync(query, cancellationToken);
			} catch (Exception ex) {
				throw new WebhookEntityException($"Could not get the subscriptions for event type '{eventType}'", ex);
			}
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
		public Task<string[]> GetEventTypesAsync(TSubscription subscription, CancellationToken cancellationToken = default) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			var eventTypes = subscription.Events.Select(x => x.EventType).ToArray();

			return Task.FromResult(eventTypes);
		}

		/// <inheritdoc/>
		public Task AddEventTypesAsync(TSubscription subscription, string[] eventTypes, CancellationToken cancellationToken = default) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			foreach (var eventType in eventTypes) {
				subscription.Events.Add(new DbWebhookSubscriptionEvent {
					EventType = eventType
				});
			}

			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		public Task RemoveEventTypesAsync(TSubscription subscription, string[] eventTypes, CancellationToken cancellationToken = default) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			foreach (var eventType in eventTypes) {
				var found = subscription.Events.FirstOrDefault(x => x.EventType == eventType);
				if (found != null)
					subscription.Events.Remove(found);
			}

			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		public Task<IDictionary<string, string>> GetHeadersAsync(TSubscription subscription, CancellationToken cancellationToken = default) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			var headers = subscription.Headers.ToDictionary(x => x.Key, x => x.Value);

			return Task.FromResult<IDictionary<string, string>>(headers);
		}

		/// <inheritdoc/>
		public Task AddHeadersAsync(TSubscription subscription, IDictionary<string, string> headers, CancellationToken cancellationToken = default) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			foreach (var header in headers) {
				subscription.Headers.Add(new DbWebhookSubscriptionHeader {
					Key = header.Key,
					Value = header.Value
				});
			}

			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		public Task RemoveHeadersAsync(TSubscription subscription, string[] headerKeys, CancellationToken cancellationToken = default) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			foreach (var headerKey in headerKeys) {
				var found = subscription.Headers.FirstOrDefault(x => x.Key == headerKey);
				if (found != null)
					subscription.Headers.Remove(found);
			}

			return Task.CompletedTask;
		}
	}
}
