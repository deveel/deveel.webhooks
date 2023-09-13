using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deveel.Webhooks {
	public class DbEventPropertyConfiguration : IEntityTypeConfiguration<DbEventProperty> {
		public virtual void Configure(EntityTypeBuilder<DbEventProperty> builder) {
			builder.ToTable("event_properties");

			builder.HasKey(x => x.Id);
			builder.Property(x => x.Id)
				.HasColumnName("id")
				.ValueGeneratedOnAdd();

			builder.Property(x => x.Key)
				.IsRequired()
				.HasColumnName("key");

			builder.Property(x => x.Value)
				.HasColumnName("value");

			builder.Property(x => x.ValueType)
				.IsRequired()
				.HasColumnName("value_type");

			builder.Property(x => x.EventId)
				.IsRequired()
				.HasColumnName("event_id");

			builder.HasOne(x => x.Event)
				.WithMany(x => x.Properties)
				.HasForeignKey(x => x.EventId);
		}
	}
}
