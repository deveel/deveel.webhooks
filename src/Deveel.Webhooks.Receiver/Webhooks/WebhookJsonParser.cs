// Copyright 2022 Deveel
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

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Deveel.Webhooks {
	public static class WebhookJsonParser {
		public static async Task<T> ParseAsync<T>(string content, JsonSerializerSettings serializerSettings, Action<JObject, T> afterRead, CancellationToken cancellationToken) {
			JToken token;

			using (var textReader = new StringReader(content)) {
				using var jsonReader = new JsonTextReader(textReader);
				token = await JToken.ReadFromAsync(jsonReader, cancellationToken);
			}

			var result = token.ToObject<T>(JsonSerializer.Create(serializerSettings));

			afterRead?.Invoke((JObject)token, result);

			return result;
		}
	}
}
