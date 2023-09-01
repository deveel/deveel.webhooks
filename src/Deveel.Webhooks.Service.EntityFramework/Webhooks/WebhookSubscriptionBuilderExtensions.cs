﻿// Copyright 2022-2023 Deveel
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

namespace Deveel.Webhooks {
    public static class WebhookSubscriptionBuilderExtensions {
        public static EntityWebhookStorageBuilder<TSubscription> UseEntityFramework<TSubscription>(this WebhookSubscriptionBuilder<TSubscription> builder)
            where TSubscription : WebhookSubscriptionEntity {
            return new EntityWebhookStorageBuilder<TSubscription>(builder);
        }

        public static WebhookSubscriptionBuilder<TSubscription> UseEntityFramework<TSubscription>(this WebhookSubscriptionBuilder<TSubscription> builder, Action<EntityWebhookStorageBuilder<TSubscription>> configure)
            where TSubscription : WebhookSubscriptionEntity {
            var storageBuilder = builder.UseEntityFramework();
            configure(storageBuilder);
            return builder;
        }
    }
}
