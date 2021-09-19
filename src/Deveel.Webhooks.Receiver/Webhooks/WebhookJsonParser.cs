using System;
using System.IO;
using System.Text;
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
