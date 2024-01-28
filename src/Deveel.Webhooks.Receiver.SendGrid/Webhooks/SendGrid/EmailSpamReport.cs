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

using System.Text.Json.Serialization;

namespace Deveel.Webhooks.SendGrid {
	/// <summary>
	/// Describes a spam report for an email message.
	/// </summary>
	public sealed class EmailSpamReport {
		/// <summary>
		/// Gets or sets the score of the spam report.
		/// </summary>
		[JsonPropertyName("score")]
		public double Score { get; set; }

		/// <summary>
		/// Gets or sets the threshold of the spam report.
		/// </summary>
		[JsonPropertyName("threshold")]
		public double? Threshold { get; set; }

		/// <summary>
		/// Gets or sets the rules that were matched to
		/// elaborate the spam report.
		/// </summary>
		[JsonPropertyName("matched_rules")]
		public IList<MatchedRule>? MatchedRules { get; set; }

		/// <summary>
		/// Gets or sets the details of the spam report.
		/// </summary>
		[JsonPropertyName("spam_report")]
		public SpamReportDetails? Details { get; set; }
	}
}
