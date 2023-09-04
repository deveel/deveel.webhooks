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
    /// The configuration of the <see cref="DbEventInfo"/> entity
    /// used to configure the database table schema to store
    /// an event.
    /// </summary>
    public class DbEventInfoConfiguration : IEntityTypeConfiguration<DbEventInfo> {
        /// <inheritdoc/>
        public virtual void Configure(EntityTypeBuilder<DbEventInfo> builder) {
            builder.ToTable("event_info");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.EventType)
                .HasColumnName("event_type")
                .IsRequired();

            builder.Property(x => x.EventId)
                .HasColumnName("event_id")
                .IsRequired();

            builder.Property(x => x.Subject)
                .HasColumnName("subject")
                .IsRequired();

            builder.Property(x => x.TimeStamp)
                .HasColumnName("timestamp")
                .IsRequired();

            builder.Property(x => x.DataVersion)
                .HasColumnName("data_version");
        }
    }
}
