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

namespace Deveel.Webhooks {
    public class WebhookDbContext : DbContext {
        public WebhookDbContext(DbContextOptions<WebhookDbContext> options) : base(options) {
        }

        public virtual DbSet<DbWebhookSubscription> Subscriptions { get; set; }

        public virtual DbSet<DbWebhookDeliveryResult> DeliveryResults { get; set; }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.ApplyConfiguration(new DbWebhookSubscriptionConfiguration());
            modelBuilder.ApplyConfiguration(new DbWebhookSubscriptionHeaderConfiguration());
            modelBuilder.ApplyConfiguration(new DbWebhookSubscriptionPropertyConfiguration());
            modelBuilder.ApplyConfiguration(new DbWebhookFilterConfiguration());
            modelBuilder.ApplyConfiguration(new DbWebhookSubscriptionEventConfiguration());

            modelBuilder.ApplyConfiguration(new DbWebhookReceiverConfiguration());
            modelBuilder.ApplyConfiguration(new DbWebhookReceiverHeaderConfiguration());
            modelBuilder.ApplyConfiguration(new DbEventInfoConfiguration());

            modelBuilder.ApplyConfiguration(new DbWebhookConfiguration());
            modelBuilder.ApplyConfiguration(new DbWebhookDeliveryResultConfiguration());
            modelBuilder.ApplyConfiguration(new DbWebhookDeliveryAttemptConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
