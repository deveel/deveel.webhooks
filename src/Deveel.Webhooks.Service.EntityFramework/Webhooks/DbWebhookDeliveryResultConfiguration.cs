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

            builder.Property(x => x.EventId)
                .HasColumnName("event_id")
                .IsRequired();

            builder.Property(x => x.ReceiverId)
                .HasColumnName("receiver_id");

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
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
