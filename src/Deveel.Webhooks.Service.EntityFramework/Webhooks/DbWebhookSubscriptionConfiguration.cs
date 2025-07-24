// Copyright 2022-2025 Antonello Provenzano
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
    /// <summary>
    /// The configuration of the <see cref="DbWebhookSubscription"/> entity
    /// </summary>
    public class DbWebhookSubscriptionConfiguration : IEntityTypeConfiguration<DbWebhookSubscription> {
        /// <inheritdoc/>
        public virtual void Configure(EntityTypeBuilder<DbWebhookSubscription> builder) {
            builder.ToTable("webhook_subscriptions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                .HasColumnName("name")
                .IsRequired();

            builder.Property(x => x.TenantId)
                .HasColumnName("tenant_id");

            builder.Property(x => x.DestinationUrl)
                .HasColumnName("destination_url")
                .IsRequired();

            builder.Property(x => x.Secret)
                .HasColumnName("secret");

            builder.Property(x => x.Format)
                .HasColumnName("format");

            builder.Property(x => x.Status)
                .HasColumnName("status")
                .IsRequired();

            builder.Property(x => x.RetryCount)
                .HasColumnName("retry_count");

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at");

            builder.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at");

            builder.HasMany(x => x.Events)
                .WithOne(x => x.Subscription)
                .HasForeignKey(x => x.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Filters)
                .WithOne(x => x.Subscription)
                .HasForeignKey(x => x.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Headers)
                .WithOne(x => x.Subscription)
                .HasForeignKey(x => x.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Properties)
                .WithOne(x => x.Subscription)
                .HasForeignKey(x => x.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
