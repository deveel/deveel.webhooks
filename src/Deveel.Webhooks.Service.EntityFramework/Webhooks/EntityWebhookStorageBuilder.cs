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

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Deveel.Webhooks {
    public sealed class EntityWebhookStorageBuilder<TSubscription> where TSubscription : WebhookSubscriptionEntity {
        private readonly WebhookSubscriptionBuilder<TSubscription> builder;
        
        internal EntityWebhookStorageBuilder(WebhookSubscriptionBuilder<TSubscription> builder) {
            this.builder = builder;

            AddDefaultStorage();
        }

        public IServiceCollection Services => builder.Services;

        private void AddDefaultStorage() {
            Services.TryAddScoped<IWebhookSubscriptionStore<TSubscription>, EntityWebhookSubscriptionStrore<TSubscription>>();
            Services.TryAddScoped<EntityWebhookSubscriptionStrore<TSubscription>>();

            if (typeof(TSubscription) == typeof(WebhookSubscriptionEntity)) {
                Services.TryAddScoped<IWebhookSubscriptionStore<WebhookSubscriptionEntity>, EntityWebhookSubscriptionStrore>();
                Services.TryAddScoped<EntityWebhookSubscriptionStrore<WebhookSubscriptionEntity>>();
                Services.AddScoped<EntityWebhookSubscriptionStrore>();
            }

            Services.TryAddScoped<IWebhookDeliveryResultStore<WebhookDeliveryResultEntity>, EntityWebhookDeliveryResultStore>();
            Services.AddScoped<EntityWebhookDeliveryResultStore>();
            Services.TryAddScoped<EntityWebhookDeliveryResultStore<WebhookDeliveryResultEntity>>();

            Services.TryAddSingleton(typeof(IWebhookEntityConverter<>), typeof(DefaultWebhookEntityConverter<>));
        }

        public EntityWebhookStorageBuilder<TSubscription> UseContext<TContext>(Action<DbContextOptionsBuilder>? options = null, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TContext : WebhookDbContext {
            if (typeof(TContext) != typeof(WebhookDbContext)) {
                Services.AddDbContext<WebhookDbContext, TContext>(options, lifetime);
            } else {
                Services.AddDbContext<WebhookDbContext>(options, lifetime);
            }

            return this;
        }

        public EntityWebhookStorageBuilder<TSubscription> UseContext(Action<DbContextOptionsBuilder>? options = null, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            => UseContext<WebhookDbContext>(options, lifetime);

        /// <summary>
        /// Registers the given type of storage to be used for
        /// storing the webhook subscriptions.
        /// </summary>
        /// <typeparam name="TStore">
        /// The type of the storage to use for storing the webhook subscriptions,
        /// that is derived from <see cref="MongoDbWebhookSubscriptionStrore"/>.
        /// </typeparam>
        /// <returns>
        /// Returns the current instance of the builder for chaining.
        /// </returns>
        public EntityWebhookStorageBuilder<TSubscription> UseSubscriptionStore<TStore>()
            where TStore : EntityWebhookSubscriptionStrore<TSubscription> {
            Services.AddScoped<IWebhookSubscriptionStore<TSubscription>, TStore>();
            Services.AddScoped<TStore>();

            return this;
        }

    }
}
