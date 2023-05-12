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

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Deveel.Webhooks {
	/// <summary>
	/// The model of a delivery attempt of a webhook that is
	/// stored in a MongoDB storage.
	/// </summary>
	public class MongoWebhookDeliveryAttempt : IWebhookDeliveryAttempt {
		/// <summary>
		/// Gets or sets the code returned by the remote endpoint
		/// when the delivery was attempted.
		/// </summary>
		[Column("response_code")]
		public int? ResponseStatusCode { get; set; }

		/// <summary>
		/// Gets or sets the message returned by the remote endpoint
		/// when the delivery was attempted.
		/// </summary>
		[Column("response_message")]
		public string? ResponseMessage { get; set; }

		/// <summary>
		/// Gets or sets the time-stamp when the delivery was attempted.
		/// </summary>
		[Column("started_at")]
		public DateTimeOffset StartedAt { get; set; }

		/// <summary>
		/// Gets or sets the time-stamp when the delivery attempt ended.
		/// </summary>
		[Column("ended_at")]
		public DateTimeOffset? EndedAt { get; set; }
	}
}
