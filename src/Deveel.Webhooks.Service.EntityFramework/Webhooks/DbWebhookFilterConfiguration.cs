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
    /// The configuration of the <see cref="DbWebhookFilter"/> entity
    /// </summary>
    public class DbWebhookFilterConfiguration : IEntityTypeConfiguration<DbWebhookFilter> {
        /// <inheritdoc/>
        public virtual void Configure(EntityTypeBuilder<DbWebhookFilter> builder) {
            builder.ToTable("webhook_filters");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.SubscriptionId)
                .HasColumnName("subscription_id")
                .IsRequired();

            builder.Property(x => x.Expression)
                .HasColumnName("expression")
                .IsRequired();

            builder.Property(x => x.Format)
                 .HasColumnName("format")
                .IsRequired()
                .HasMaxLength(255);

            builder.HasOne(x => x.Subscription)
                .WithMany(x => x.Filters)
                .HasForeignKey(x => x.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
