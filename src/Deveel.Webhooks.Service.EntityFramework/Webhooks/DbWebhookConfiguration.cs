using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deveel.Webhooks {
	/// <summary>
	/// Configures the database entity that represents a webhook
	/// </summary>
    public class DbWebhookConfiguration : IEntityTypeConfiguration<DbWebhook> {
		/// <inheritdoc/>
        public virtual void Configure(EntityTypeBuilder<DbWebhook> builder) {
            builder.ToTable("webhooks");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.WebhookId)
                .HasColumnName("webhook_id");

            builder.Property(x => x.EventType)
                .HasColumnName("event_type")
                .IsRequired();

            builder.Property(x => x.TimeStamp)
                .HasColumnName("timestamp")
                .IsRequired();

            builder.Property(x => x.Data)
                .HasColumnName("data");

			builder.HasIndex(x => x.WebhookId);
        
            builder.HasIndex(x => x.EventType);
        }
    }
}
