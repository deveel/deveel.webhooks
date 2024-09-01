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
	/// The configuration of the <see cref="DbWebhookDeliveryResult"/> entity
	/// </summary>
    public class DbWebhookDeliveryResultConfiguration : IEntityTypeConfiguration<DbWebhookDeliveryResult> {
		/// <inheritdoc/>
        public virtual void Configure(EntityTypeBuilder<DbWebhookDeliveryResult> builder) {
            builder.ToTable("webhook_delivery_results");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.OperationId)
                .HasColumnName("operation_id")
                .HasMaxLength(36)
                .IsRequired();

            builder.Property(x => x.TenantId)
                .HasColumnName("tenant_id");

            builder.Property(x => x.WebhookId)
                .HasColumnName("webhook_id")
                .IsRequired();

            builder.Property(x => x.EventId)
                .HasColumnName("event_id")
                .IsRequired(false);

            builder.Property(x => x.ReceiverId)
                .HasColumnName("receiver_id")
                .IsRequired(false);

            builder.HasOne(x => x.EventInfo)
                .WithMany()
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.Receiver)
                .WithMany()
                .HasForeignKey(x => x.ReceiverId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(x => x.DeliveryAttempts)
                .WithOne(x => x.DeliveryResult)
                .HasForeignKey(x => x.DeliveryResultId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Webhook)
                .WithMany()
                .HasForeignKey(x => x.WebhookId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
