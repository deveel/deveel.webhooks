using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace System.Net.Http {
	public static class HttpContentExtensions {
		public static Task<T> ReadAsObjectAsync<T>(this HttpContent content, Action<JObject, T> afterRead, CancellationToken cancellationToken)
			where T : class
			=> content.ReadAsObjectAsync(new JsonSerializerSettings(), afterRead, cancellationToken);

		public static Task<T> ReadAsObjectAsync<T>(this HttpContent content, JsonSerializerSettings serializerSettings, CancellationToken cancellationToken)
			where T : class
			=> content.ReadAsObjectAsync<T>(serializerSettings, null, cancellationToken);

		public static Task<T> ReadAsObjectAsync<T>(this HttpContent content, JsonSerializerSettings serializerSettings, Action<JObject, T> afterRead)
			where T : class
			=> content.ReadAsObjectAsync(serializerSettings, afterRead, default);

		public static Task<T> ReadAsObjectAsync<T>(this HttpContent content, JsonSerializerSettings serializerSettings)
			where T : class
			=> content.ReadAsObjectAsync<T>(serializerSettings, default(CancellationToken));

		public static Task<T> ReadAsObjectAsync<T>(this HttpContent content, CancellationToken cancellationToken)
			where T : class
			=> content.ReadAsObjectAsync<T>(new JsonSerializerSettings(), cancellationToken);

		public static async Task<T> ReadAsObjectAsync<T>(this HttpContent content, JsonSerializerSettings serializerSettings, Action<JObject, T> afterRead, CancellationToken cancellationToken)
			where T : class {

			serializerSettings = serializerSettings ?? new JsonSerializerSettings();

			var obj = await content.ReadAsJsonObjectAsync(cancellationToken);

			T result;

			try {
				result = obj.ToObject<T>(JsonSerializer.Create(serializerSettings));
			} catch (Exception ex) {
				throw new FormatException("The webhook JSON format is invalid", ex);
			}

			afterRead?.Invoke(obj, result);

			return result;
		}

		public static async Task<JObject> ReadAsJsonObjectAsync(this HttpContent content, CancellationToken cancellationToken = default) {
			var token = await content.ReadAsJsonAsync(cancellationToken);

			if (token.Type != JTokenType.Object)
				throw new FormatException("The json request is invalid");

			return (JObject)token;
		}

		public static async Task<JToken> ReadAsJsonAsync(this HttpContent content, CancellationToken cancellationToken = default) {
			if (content.Headers == null)
				throw new FormatException("Missing content headers");
			if (content.Headers.ContentType == null)
				throw new FormatException("Content-type of the request is missing");

			if (content.Headers.ContentType.MediaType != "application/json" &&
				content.Headers.ContentType.MediaType != "text/json")
				throw new NotSupportedException("Only JSON webhooks supported at this moment");

			// TODO: retrieve the encoding from the headers

			JToken token;

			using (var stream = await content.ReadAsStreamAsync()) {
				using (var textReader = new StreamReader(stream, Encoding.UTF8)) {
					using (var jsonReader = new JsonTextReader(textReader)) {
						token = await JToken.ReadFromAsync(jsonReader, cancellationToken);
					}
				}
			}

			return token;
		}

		public static Task<object> ReadAsObjectAsync(this HttpContent content, Type webhookType, JsonSerializerSettings serializerSettings, Action<JObject, object> afterRead)
			=> content.ReadAsObjectAsync(webhookType, serializerSettings, afterRead, default);

		public static Task<object> ReadAsObjectAsync(this HttpContent content, Type webhookType, JsonSerializerSettings serializerSettings, CancellationToken cancellationToken)
			=> content.ReadAsObjectAsync(webhookType, serializerSettings, null, cancellationToken);

		public static Task<object> ReadAsObjectAsync(this HttpContent content, Type webhookType, CancellationToken cancellationToken)
			=> content.ReadAsObjectAsync(webhookType, new JsonSerializerSettings(), cancellationToken);

		public static Task<object> ReadAsObjectAsync(this HttpContent content, Type webhookType, Action<JObject, object> afterRead, CancellationToken cancellationToken)
			=> content.ReadAsObjectAsync(webhookType, new JsonSerializerSettings(), afterRead, cancellationToken);

		public static Task<object> ReadAsObjectAsync(this HttpContent content, Type webhookType, Action<JObject, object> afterRead)
			=> content.ReadAsObjectAsync(webhookType, afterRead, default);

		public static async Task<object> ReadAsObjectAsync(this HttpContent content, Type webhookType, JsonSerializerSettings serializerSettings, Action<JObject, object> afterRead, CancellationToken cancellationToken) {
			serializerSettings = serializerSettings ?? new JsonSerializerSettings();

			var obj = await content.ReadAsJsonObjectAsync(cancellationToken);

			object result;

			try {
				result = obj.ToObject(webhookType, JsonSerializer.Create(serializerSettings));
			} catch (Exception ex) {
				throw new FormatException("The webhook JSON format is invalid", ex);
			}

			afterRead?.Invoke(obj, result);

			return result;
		}
	}
}
