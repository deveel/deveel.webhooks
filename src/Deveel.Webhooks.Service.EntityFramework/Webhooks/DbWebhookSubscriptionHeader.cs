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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Deveel.Webhooks {
	/// <summary>
	/// Describes an header that is associated to a webhook subscription.
	/// </summary>
    public class DbWebhookSubscriptionHeader {
		/// <summary>
		/// Gets or sets the database identifier of the 
		/// header entity.
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
		/// Gets or sets the database entity that represents
		/// the subscription that owns the header.
		/// </summary>
        public virtual DbWebhookSubscription? Subscription { get; set; }

		/// <summary>
		/// Gets or sets the database identifier of the
		/// webhook subscription that owns the header.
		/// </summary>
        public string? SubscriptionId { get; set; }
    }
}
