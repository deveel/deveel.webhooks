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

using Microsoft.Extensions.Logging;

namespace Deveel.Webhooks {
    /// <summary>
    /// A default implementation of <see cref="IDbWebhookConverter{TWebhook}"/> that
    /// stores a <see cref="DbWebhookSubscription"/> in the database.
    /// </summary>
    public class EntityWebhookSubscriptionRepository : EntityWebhookSubscriptionRepository<DbWebhookSubscription> {
        /// <inheritdoc/>
        public EntityWebhookSubscriptionRepository(WebhookDbContext context, ILogger<EntityWebhookSubscriptionRepository>? logger = null) 
			: base(context, logger) {
        }
    }
}
