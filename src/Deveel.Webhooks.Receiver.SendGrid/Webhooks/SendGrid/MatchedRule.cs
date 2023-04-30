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

using System.Text.Json.Serialization;

namespace Deveel.Webhooks.SendGrid {
	/// <summary>
	/// Describes a rule that was matched by the SendGrid
	/// to provide a spam report for an email.
	/// </summary>
	public readonly struct MatchedRule {
		/// <summary>
		/// Constructs the rule with the given name,
		/// score and description.
		/// </summary>
		/// <param name="name">
		/// The name of the rule that was matched.
		/// </param>
		/// <param name="score">
		/// The sceore of the rule.
		/// </param>
		/// <param name="description">
		/// A description of the rule.
		/// </param>
		[JsonConstructor]
		public MatchedRule(string name, double? score = null, string? description = null) {
			Name = name;
			Score = score;
			Description = description;
		}

		/// <summary>
		/// Gets the name of the rule that was matched.
		/// </summary>
		[JsonPropertyName("name")]
		public string Name { get; }

		/// <summary>
		/// Gets the description of the rule that was matched.
		/// </summary>
		[JsonPropertyName("description")]
		public string? Description { get; }

		/// <summary>
		/// Gets the score of the rule that was matched.
		/// </summary>
		[JsonPropertyName("score")]
		public double? Score { get; }
	}
}
