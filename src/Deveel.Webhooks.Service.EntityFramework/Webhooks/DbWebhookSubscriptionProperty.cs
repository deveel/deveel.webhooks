// Copyright 2022-2025 Antonello Provenzano
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
	/// Represents a property that is associated to a 
	/// webhook subscription.
	/// </summary>
    public class DbWebhookSubscriptionProperty {
		/// <summary>
		/// Gets or sets the database identifier of the property entity.
		/// </summary>
        public int? Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the property.
		/// </summary>
        public string Key { get; set; }

		/// <summary>
		/// Gets or sets the value of the property.
		/// </summary>
        public string? Value { get; set; }

		/// <summary>
		/// Gets or sets the type of the value of the property.
		/// </summary>
        public string ValueType { get; set; }

		/// <summary>
		/// Gets or sets the database entity that describes the subscription
		/// that owns the property.
		/// </summary>
        public virtual DbWebhookSubscription? Subscription { get; set; }

		/// <summary>
		/// Gets or sets the database identifier of the subscription
		/// that owns the property.
		/// </summary>
        public string? SubscriptionId { get; set; }
    }
}
