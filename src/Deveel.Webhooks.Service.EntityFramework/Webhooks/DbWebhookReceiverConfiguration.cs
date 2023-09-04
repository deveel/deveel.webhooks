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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deveel.Webhooks {
    /// <summary>
    /// The configuration of the <see cref="DbWebhookReceiver"/> entity
    /// </summary>
    public class DbWebhookReceiverConfiguration : IEntityTypeConfiguration<DbWebhookReceiver> {
        /// <inheritdoc/>
        public virtual void Configure(EntityTypeBuilder<DbWebhookReceiver> builder) {
            builder.ToTable("webhook_receivers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.SubscriptionName)
                .HasColumnName("subscription_name");


            builder.Property(x => x.DestinationUrl)
                .HasColumnName("destination_url")
                .IsRequired();

            builder.Property(x => x.BodyFormat)
                .HasColumnName("body_format")
                .HasMaxLength(50);

            builder.Property(x => x.SubscriptionId)
                .HasColumnName("subscription_id");

            builder.HasOne(x => x.Subscription)
                .WithMany()
                .HasForeignKey(x => x.SubscriptionId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(x => x.Headers)
                .WithOne(x => x.Receiver)
                .HasForeignKey(x => x.ReceiverId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
