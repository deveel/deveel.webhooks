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

using System.Text.Json.Serialization;

namespace Deveel.Webhooks.SendGrid {
	/// <summary>
	/// Provides the details about a rule used to evaluate
	/// a spam report.
	/// </summary>
	public sealed class SpamRuleDetails {
		/// <summary>
		/// Gets or sets the description of the rule.
		/// </summary>
		[JsonPropertyName("description")]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the name of the rule.
		/// </summary>
		[JsonPropertyName("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the score of the rule.
		/// </summary>
		[JsonPropertyName("score")]
		public double Score { get; set; }
	}
}
