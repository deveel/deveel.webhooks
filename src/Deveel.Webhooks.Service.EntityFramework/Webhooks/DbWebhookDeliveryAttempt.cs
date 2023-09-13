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

namespace Deveel.Webhooks {
	/// <summary>
	/// An implementation of <see cref="IWebhookDeliveryAttempt"/> that
	/// is used to store the delivery attempts in a database.
	/// </summary>
    public class DbWebhookDeliveryAttempt : IWebhookDeliveryAttempt {
		/// <summary>
		/// Gets or sets the identifier of the delivery attempt
		/// in the database.
		/// </summary>
        public int? Id { get; set; }

		/// <inheritdoc/>
        public int? ResponseStatusCode { get; set; }

		/// <inheritdoc/>
        public string? ResponseMessage { get; set; }

		/// <inheritdoc/>
        public DateTimeOffset StartedAt { get; set; }

		/// <inheritdoc/>
        public DateTimeOffset? EndedAt { get; set; }

		/// <summary>
		/// Gets or sets a reference to the <see cref="DbWebhookDeliveryResult"/>
		/// that represents the scope of the delivery attempt.
		/// </summary>
        public int? DeliveryResultId { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="DbWebhookDeliveryResult"/> that
		/// represents the scope of the delivery attempt.
		/// </summary>
        public virtual DbWebhookDeliveryResult DeliveryResult { get; set; }
    }
}
