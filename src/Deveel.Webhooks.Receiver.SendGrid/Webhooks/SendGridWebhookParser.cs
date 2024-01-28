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

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Deveel.Webhooks {
	static class SendGridWebhookParser {
		static SendGridWebhookParser() {
			Options = new JsonSerializerOptions {
				PropertyNameCaseInsensitive = false,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
			};

			Options.Converters.Add(new JsonStringEnumMemberConverter(JsonNamingPolicy.CamelCase));
		}

		public static JsonSerializerOptions Options { get; }
	}
}
