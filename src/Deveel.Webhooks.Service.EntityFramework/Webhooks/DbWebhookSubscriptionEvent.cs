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

// Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618

namespace Deveel.Webhooks {
	/// <summary>
	/// Describes an event that is listened by a webhook subscription.
	/// </summary>
    public class DbWebhookSubscriptionEvent {
		/// <summary>
		/// Gets or sets the database identifier of the event entity.
		/// </summary>
        public int? Id { get; set; }

		/// <summary>
		/// Gets or sets the type of the event that is listened by 
		/// the subscription.
		/// </summary>
        public string EventType { get; set; }

		/// <summary>
		/// Gets or sets the database identifier of the subscription
		/// that listens the event.
		/// </summary>
        public string? SubscriptionId { get; set; }

		/// <summary>
		/// Gets or sets the database entity that describes the subscription
		/// that listens the event.
		/// </summary>
        public virtual DbWebhookSubscription? Subscription { get; set; }
    }
}
