// Copyright 2022-2024 Antonello Provenzano
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
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Deveel.Webhooks {
	/// <summary>
	/// An implementation of a <see cref="DbContext"/> that provides
	/// schema and mapping for the storage system.
	/// </summary>
    public class WebhookDbContext : DbContext {
		/// <inheritdoc/>
        public WebhookDbContext(DbContextOptions<WebhookDbContext> options) : base(options) {
        }

		/// <summary>
		/// Gets or sets the set a <c>DbSet</c> that provides query access to
		/// the <see cref="DbWebhookSubscription"/> entities.
		/// </summary>
        public virtual DbSet<DbWebhookSubscription> Subscriptions { get; set; }

		/// <summary>
		/// Gets or sets the set a <c>DbSet</c> that provides query access to
		/// the <see cref="DbWebhookDeliveryResult"/> entities.
		/// </summary>
        public virtual DbSet<DbWebhookDeliveryResult> DeliveryResults { get; set; }

        /// <summary>
        /// Gets or sets the set a <c>DbSet</c> that provides query access to
        /// the <see cref="DbWebhookDeliveryAttempt"/> entities.
        /// </summary>
        public virtual DbSet<DbWebhook> Webhooks { get; set; }

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
