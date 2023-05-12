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


using System.ComponentModel.DataAnnotations.Schema;

namespace Deveel.Webhooks {
	/// <summary>
	/// Represents a webhook filter that is stored in a MongoDB storage.
	/// </summary>
	public class MongoWebhookFilter : IWebhookFilter {
		/// <summary>
		/// Gets or sets the expression used to filter the webhook
		/// to be delivered to a receiver.
		/// </summary>
		[Column("expression")]
		public string Expression { get; set; }

		/// <summary>
		/// Gets or sets the format of the expression
		/// </summary>
		[Column("format")]
		public string Format { get; set; }
	}
}
