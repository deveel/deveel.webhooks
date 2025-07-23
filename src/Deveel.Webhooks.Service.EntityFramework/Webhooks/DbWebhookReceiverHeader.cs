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

using Microsoft.EntityFrameworkCore.Infrastructure;

// Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618

namespace Deveel.Webhooks {
	/// <summary>
	/// Describes the entity that represents an header
	/// that is associated to a webhook receiver.
	/// </summary>
    public class DbWebhookReceiverHeader {
		/// <summary>
		/// Gets or sets the database identifier of the header entity.
		/// </summary>
        public int? Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the header.
		/// </summary>
        public string Key { get; set; }

		/// <summary>
		/// Gets or sets the value of the header.
		/// </summary>
        public string Value { get; set; }

		/// <summary>
		/// Gets or sets the database entity that describes 
		/// the receiver that owns the header.
		/// </summary>
        public virtual DbWebhookReceiver? Receiver { get; set; }

		/// <summary>
		/// Gets or sets the database identifier of the receiver
		/// that owns the header.
		/// </summary>
        public int? ReceiverId { get; set; }
    }
}
