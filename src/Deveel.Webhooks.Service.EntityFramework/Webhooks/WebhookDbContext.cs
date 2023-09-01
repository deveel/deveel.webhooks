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
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deveel.Webhooks {
    public class WebhookDbContext : DbContext {
        public WebhookDbContext(DbContextOptions<WebhookDbContext> options) : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            // WebhookSubscriptionEntity
            modelBuilder.Entity<WebhookSubscriptionEntity>()
                .HasMany(x => x.Events)
                .WithOne(x => x.Subscription)
                .HasForeignKey(x => x.SubscriptionId)
                .HasConstraintName("fk_webhooksub_events");

            modelBuilder.Entity<WebhookSubscriptionEntity>()
                .HasMany(x => x.Headers)
                .WithOne(x => x.Subscription)
                .HasForeignKey(x => x.SubscriptionId)
                .HasConstraintName("fk_webhooksubs_headers");

            modelBuilder.Entity<WebhookSubscriptionEntity>()
                .HasMany(x => x.Properties)
                .WithOne(x => x.Subscription)
                .HasForeignKey(x => x.SubscriptionId)
                .HasConstraintName("fk_webhooksubs_props");

            modelBuilder.Entity<WebhookSubscriptionEntity>()
                .HasMany(x => x.Filters)
                .WithOne(x => x.Subscription)
                .HasForeignKey(x => x.SubscriptionId)
                .HasConstraintName("fk_webhooksubs_filters");

            // WebhookSubscriptionHeader
            modelBuilder.Entity<WebhookSubscriptionHeader>();

            modelBuilder.Entity<WebhookSubscriptionHeader>()
                .HasOne(x => x.Subscription)
                .WithMany(x => x.Headers)
                .HasForeignKey(x => x.SubscriptionId)
                .HasConstraintName("fk_webhookheader_sub");

            // WebhookSubscriptionProperty
            modelBuilder.Entity<WebhookSubscriptionProperty>();

            modelBuilder.Entity<WebhookSubscriptionProperty>()
                .HasOne(x => x.Subscription)
                .WithMany(x => x.Properties)
                .HasForeignKey(x => x.SubscriptionId)
                .HasConstraintName("fk_webhookprop_sub");

            // WebhookFilterEntity
            modelBuilder.Entity<WebhookFilterEntity>();

            modelBuilder.Entity<WebhookFilterEntity>()
                .HasOne(x => x.Subscription)
                .WithMany(x => x.Filters)
                .HasForeignKey(x => x.SubscriptionId)
                .HasConstraintName("fk_webhookfilter_sub");

            // WebhookEventSubscription
            modelBuilder.Entity<WebhookEventSubscription>();

            modelBuilder.Entity<WebhookEventSubscription>()
                .HasOne(x => x.Subscription)
                .WithMany(x => x.Events)
                .HasForeignKey(x => x.SubscriptionId)
                .HasConstraintName("fk_webhookevent_sub");

            // WebhookReceiverEntity
            modelBuilder.Entity<WebhookReceiverEntity>();

            modelBuilder.Entity<WebhookReceiverEntity>()
                .HasOne(x => x.Subscription)
                .WithMany()
                .IsRequired(false)
                .HasForeignKey(x => x.SubscriptionId)
                .HasConstraintName("fk_webhookreceiver_sub");

            modelBuilder.Entity<WebhookReceiverEntity>()
                .HasMany(x => x.Headers)
                .WithOne(x => x.Receiver)
                .HasForeignKey(x => x.ReceiverId)
                .HasConstraintName("fk_webhookreceiver_headers");

            // WebhookReceiverHeader
            modelBuilder.Entity<WebhookReceiverHeader>();

            modelBuilder.Entity<WebhookReceiverHeader>()
                .HasOne(x => x.Receiver)
                .WithMany(x => x.Headers)
                .HasForeignKey(x => x.ReceiverId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_webhookreceiverheader_receiver");

            // EventInfoEntity

            modelBuilder.Entity<EventInfoEntity>();

            // WebhookDeliveryAttemptEntity

            modelBuilder.Entity<WebhookDeliveryAttemptEntity>();

            modelBuilder.Entity<WebhookDeliveryAttemptEntity>()
                .HasOne(x => x.DeliveryResult)
                .WithMany(x => x.DeliveryAttempts)
                .HasForeignKey(x => x.DeliveryResultId)
                .HasConstraintName("fk_webhookattempt_result")
                .OnDelete(DeleteBehavior.Cascade);

            // WebhookDeliveryResultEntity

            modelBuilder.Entity<WebhookDeliveryResultEntity>();

            modelBuilder.Entity<WebhookDeliveryResultEntity>()
                .HasOne(x => x.Webhook)
                .WithMany()
                .IsRequired(false)
                .HasForeignKey(x => x.WebhookId)
                .HasConstraintName("fk_webhookresult_webhook");

            modelBuilder.Entity<WebhookDeliveryResultEntity>()
                .HasOne(x => x.EventInfo)
                .WithMany()
                .HasForeignKey(x => x.EventInfoId)
                .HasConstraintName("fk_webhookresult_event");

            modelBuilder.Entity<WebhookDeliveryResultEntity>()
                .HasOne(x => x.Receiver)
                .WithMany()
                .HasForeignKey(x => x.ReceiverId)
                .HasConstraintName("fk_webhookresult_receiver");

            base.OnModelCreating(modelBuilder);
        }
    }
}
