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

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides a mechanism to cache of anonymous types created
	/// from webhook instances
	/// </summary>
	/// <remarks>
	/// The goal of this cache is to reduce the creation of anonymous types
	/// during the filtering process in a notification of a webhook.
	/// </remarks>
	public sealed class WebhookTypeCache {
		// TODO: use another type of cache that can be configured to evict
		private readonly ConcurrentDictionary<object, object?> cache = new ConcurrentDictionary<object, object?>();
		private Func<object, object> keyGenerator;

		/// <summary>
		/// Initializes a new instance of the <see cref="WebhookTypeCache"/> class
		/// with a custom key generator.
		/// </summary>
		/// <param name="keyGenerator">
		/// A function that generates a key for a generated object to be cached.
		/// </param>
		public WebhookTypeCache(Func<object, object> keyGenerator) {
			this.keyGenerator = keyGenerator;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WebhookTypeCache"/> class
		/// that uses the hash code of the webhook instance as key.	
		/// </summary>
		public WebhookTypeCache()
			: this(obj => obj.GetHashCode()) {
		}

		/// <summary>
		/// Gets the default instance of the cache.
		/// </summary>
		public static readonly WebhookTypeCache Default = new WebhookTypeCache();

		private object ComputeKey(object obj) {
			return keyGenerator(obj);
		}

		/// <summary>
		/// Tries to get the cached object for the given webhook.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of the webhook to get the cached object for.
		/// </typeparam>
		/// <param name="webhook">
		/// The instance of the webhook to get the cached object for.
		/// </param>
		/// <param name="obj">
		/// An anonymous object that was created from the webhook and then cached,
		/// if found in the cache, otherwise <c>null</c>.
		/// </param>
		/// <returns>
		/// Returns <c>true</c> if an anonymous object was found in the cache for
		/// the given webhook, otherwise <c>false</c>.
		/// </returns>
		public bool TryGet<TWebhook>(TWebhook? webhook, [MaybeNullWhen(false)] out object? obj) {
			if (webhook == null) {
				obj = null;
				return false;
			}

			var key = ComputeKey(webhook);
			if (!cache.TryGetValue(key, out var value)) {
				obj = null;
				return false;
			}

			obj = value;
			return true;
		}

		/// <summary>
		/// Gets the cached object for the given webhook,
		/// or creates a new one if not found in the cache.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of the webhook to get the cached object for.
		/// </typeparam>
		/// <param name="webhook">
		/// The instance of the webhook to get the cached object for.
		/// </param>
		/// <param name="creator">
		/// A function that creates a new anonymous object from the given webhook.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used to cancel the creation of the
		/// anonymous object.
		/// </param>
		/// <returns></returns>
		public async Task<object?> GetOrCreateAsync<TWebhook>(TWebhook? webhook, Func<TWebhook, CancellationToken, Task<object?>> creator, CancellationToken cancellationToken = default) {
			if (webhook == null)
				return null;

			if (!TryGet(webhook, out var obj)) {
				obj = await creator(webhook, cancellationToken);

				var key = ComputeKey(webhook);
				cache[key] = obj;
			}

			return obj;
		}

		/// <summary>
		/// Gets the cached object for the given webhook,
		/// or creates a new one if not found in the cache.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of the webhook to get the cached object for.
		/// </typeparam>
		/// <param name="webhook">
		/// The instance of the webhook to get the cached object for.
		/// </param>
		/// <param name="creator">
		/// The function that creates a new anonymous object from the given webhook.
		/// </param>
		/// <returns>
		/// Returns the cached object for the given webhook, or a new one if not
		/// found in the cache.
		/// </returns>
		public async Task<object?> GetOrCreateAsync<TWebhook>(TWebhook? webhook, Func<TWebhook, Task<object?>> creator)
			=> await GetOrCreateAsync(webhook, (w, _) => creator(w));

		/// <summary>
		/// Gets the cached object for the given webhook,
		/// or creates a new one if not found in the cache.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of the webhook to get the cached object for.
		/// </typeparam>
		/// <param name="webhook">
		/// The webhook instance to get the cached object for.
		/// </param>
		/// <param name="creator">
		/// The function that creates a new anonymous object from the given webhook.
		/// </param>
		/// <returns>
		/// Returns the cached object for the given webhook, or a new one if not
		/// found in the cache.
		/// </returns>
		public object? GetOrCreate<TWebhook>(TWebhook? webhook, Func<TWebhook, object?> creator) {
			if (webhook == null)
				return null;

			if (!TryGet(webhook, out var obj)) {
				obj = creator(webhook);
				var key = ComputeKey(webhook);
				cache[key] = obj;
			}
			return obj;
		}
	}
}
