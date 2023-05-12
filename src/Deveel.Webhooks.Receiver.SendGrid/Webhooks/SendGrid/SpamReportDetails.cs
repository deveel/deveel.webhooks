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

using System.Text.Json.Serialization;

namespace Deveel.Webhooks.SendGrid {
	public sealed class SpamReportDetails {
		[JsonPropertyName("verdict")]
		public string Verdict { get; set; }

		[JsonPropertyName("score")]
		public double Score { get; set; }

		[JsonPropertyName("threshold")]
		public double Threshold { get; set; }

		[JsonPropertyName("policy")]
		public string Policy { get; set; }

		[JsonPropertyName("bulkiness")]
		public string Bulkiness { get; set; }

		[JsonPropertyName("rules")]
		public IList<SpamRuleDetails> Rules { get; set; }
	}
}
