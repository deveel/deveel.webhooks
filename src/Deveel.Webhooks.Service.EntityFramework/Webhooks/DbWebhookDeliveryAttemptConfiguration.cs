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
	/// <summary>
	/// Configures the database entity that represents a webhook
	/// delivery attempt.
	/// </summary>
    public class DbWebhookDeliveryAttemptConfiguration : IEntityTypeConfiguration<DbWebhookDeliveryAttempt> {
		/// <inheritdoc/>
        public virtual void Configure(EntityTypeBuilder<DbWebhookDeliveryAttempt> builder) {
            builder.ToTable("webhook_delivery_attempts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ResponseStatusCode)
                .HasColumnName("response_status_code");

            builder.Property(x => x.ResponseMessage)
                .HasColumnName("response_message");

            builder.Property(x => x.StartedAt)
                .HasColumnName("started_at")
                .IsRequired();

            builder.Property(x => x.EndedAt)
                .HasColumnName("ended_at");

            builder.Property(x => x.DeliveryResultId)
                .HasColumnName("delivery_result_id");

            builder.HasOne(x => x.DeliveryResult)
                .WithMany(x => x.DeliveryAttempts)
                .HasForeignKey(x => x.DeliveryResultId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
