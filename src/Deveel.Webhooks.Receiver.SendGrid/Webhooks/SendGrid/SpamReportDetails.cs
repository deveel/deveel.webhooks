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

// Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618

using System.Text.Json.Serialization;

namespace Deveel.Webhooks.SendGrid {
	/// <summary>
	/// Provides details about the spam report from
	/// the SendGrid system.
	/// </summary>
	public sealed class SpamReportDetails {
		/// <summary>
		/// Gets the verdict of the spam report.
		/// </summary>
		[JsonPropertyName("verdict")]
		public string Verdict { get; set; }

		/// <summary>
		/// Gets the score of the spam report.
		/// </summary>
		[JsonPropertyName("score")]
		public double Score { get; set; }

		/// <summary>
		/// Gets the threshold of the spam report.
		/// </summary>
		[JsonPropertyName("threshold")]
		public double Threshold { get; set; }

		/// <summary>
		/// Gets the policy used to evaluate the spam report.
		/// </summary>
		[JsonPropertyName("policy")]
		public string Policy { get; set; }

		/// <summary>
		/// Gets the bulkiness of the spam report.
		/// </summary>
		[JsonPropertyName("bulkiness")]
		public string Bulkiness { get; set; }

		/// <summary>
		/// Gets the rules used to evaluate the spam report.
		/// </summary>
		[JsonPropertyName("rules")]
		public IList<SpamRuleDetails> Rules { get; set; }
	}
}
