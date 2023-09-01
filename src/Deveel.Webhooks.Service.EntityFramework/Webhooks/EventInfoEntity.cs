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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Deveel.Webhooks {
    [Table("event_info")]
    public class EventInfoEntity : IEventInfo {
        [Key, Column("id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        [Required, Column("subject")]
        public string Subject { get; set; }

        [Required, Column("event_type")]
        public string EventType { get; set;}

        [Required, Column("event_id")]
        public string EventId { get; set; }

        string IEventInfo.Id => EventId;

        [Required, Column("timestamp")]
        public DateTimeOffset TimeStamp { get; set; }

        [Column("data_version")]
        public string? DataVersion { get; set; }

        // TODO: Convert from JSON
        object? IEventInfo.Data => Data;

        [Column("data")]
        public string? Data { get; set; }
    }
}
