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
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deveel.Webhooks {
    /// <summary>
    /// The configuration of the <see cref="DbWebhookSubscriptionHeader"/> entity
    /// </summary>
    public class DbWebhookSubscriptionHeaderConfiguration : IEntityTypeConfiguration<DbWebhookSubscriptionHeader> {
        /// <inheritdoc/>
        public virtual void Configure(EntityTypeBuilder<DbWebhookSubscriptionHeader> builder) {
            builder.ToTable("webhook_subscription_headers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.SubscriptionId)
                .HasColumnName("subscription_id")
                .IsRequired();

            builder.Property(x => x.Key)
                .HasColumnName("key")
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Value)
                .HasColumnName("value")
                .IsRequired()
                .HasMaxLength(255);

            builder.HasOne(x => x.Subscription)
                .WithMany(x => x.Headers)
                .HasForeignKey(x => x.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
