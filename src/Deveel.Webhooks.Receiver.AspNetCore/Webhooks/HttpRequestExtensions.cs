using System;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using System.Xml.Linq;

using Microsoft.AspNetCore.Http;

using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.Net.Http;

namespace Deveel.Webhooks {
	public static class HttpRequestExtensions {
		public static string GetCharset(this HttpRequest request) {
			var contentType = request.ContentType;
			if (String.IsNullOrWhiteSpace(contentType))
				return null;

			var parts = contentType.Split(';', StringSplitOptions.RemoveEmptyEntries);
			var charsetPart = parts.FirstOrDefault(x => x.StartsWith("chartset"));
			if (String.IsNullOrWhiteSpace(charsetPart))
				return null;

			var index = charsetPart.IndexOf('=');
			return charsetPart.Substring(index + 1);
		}

		public static string GetMediaType(this HttpRequest request) {
			var contentType = request.ContentType;
			if (String.IsNullOrWhiteSpace(contentType))
				return null;

			int sepIndex = -1;
			if ((sepIndex = contentType.IndexOf(';')) != -1) {
				contentType = contentType.Substring(0, sepIndex);
			}

			return contentType;
		}

		public static Task<string> ReadAsStringAsync(this HttpRequest request) {
			var encoding = Encoding.UTF8;
			var charset = request.GetCharset();

			if (!String.IsNullOrWhiteSpace(charset)) {
				encoding = Encoding.GetEncoding(charset);
			}

			using (var reader = new StreamReader(request.Body, encoding)) {
				return reader.ReadToEndAsync();
			}
		}

		public static async Task<T> ReadAsObjectAsync<T>(this HttpRequest request, JsonSerializerSettings serializerSettings, Action<JObject, T> afterRead, CancellationToken cancellationToken = default) {
			serializerSettings = serializerSettings ?? new JsonSerializerSettings();

			var obj = await request.ReadAsJsonObjectAsync(cancellationToken);

			T result;

			try {
				result = obj.ToObject<T>(JsonSerializer.Create(serializerSettings));
			} catch (Exception ex) {
				throw new FormatException("The webhook JSON format is invalid", ex);
			}

			afterRead?.Invoke(obj, result);

			return result;

		}

		public static async Task<JObject> ReadAsJsonObjectAsync(this HttpRequest request, CancellationToken cancellationToken = default) {
			var token = await request.ReadAsJsonAsync(cancellationToken);

			if (token.Type != JTokenType.Object)
				throw new FormatException("The json request is invalid");

			return (JObject)token;
		}

		public static async Task<JToken> ReadAsJsonAsync(this HttpRequest request, CancellationToken cancellationToken = default) {
			if (request.Headers == null)
				throw new FormatException("Missing content headers");
				

			var mediaType = request.GetMediaType();

			if (String.IsNullOrWhiteSpace(mediaType))
				throw new FormatException("Content-type of the request is missing");

			if (mediaType != "application/json" &&
				mediaType != "text/json")
				throw new NotSupportedException("Only JSON webhooks supported at this moment");

			var encoding = Encoding.UTF8;
			var charset = request.GetCharset();

			if (!String.IsNullOrWhiteSpace(charset)) {
				encoding = Encoding.GetEncoding(charset);
			}

			JToken token;

			using (var textReader = new StreamReader(request.Body, encoding)) {
				using (var jsonReader = new JsonTextReader(textReader)) {
					token = await JToken.ReadFromAsync(jsonReader, cancellationToken);
				}
			}

			return token;
		}


		public static Task<T> GetWebhookAsync<T>(this HttpRequest request, Action<JObject, T> afterRead, CancellationToken cancellationToken = default)
			where T : class
			=> request.GetWebhookAsync(new WebhookReceiveOptions(), afterRead, cancellationToken);

		public static Task<T> GetWebhookAsync<T>(this HttpRequest request, WebhookReceiveOptions options, CancellationToken cancellationToken = default)
			where T : class
			=> request.GetWebhookAsync<T>(options, null, cancellationToken);

		public static Task<T> GetWebhookAsync<T>(this HttpRequest request, CancellationToken cancellationToken)
			where T : class
			=> request.GetWebhookAsync<T>(new WebhookReceiveOptions(), cancellationToken);

		public static Task<T> GetWebhookAsync<T>(this HttpRequest request)
			where T : class
			=> request.GetWebhookAsync<T>(default(CancellationToken));

		public static async Task<T> GetWebhookAsync<T>(this HttpRequest request, WebhookReceiveOptions options, Action<JObject, T> afterRead, CancellationToken cancellationToken)
			where T : class {
			if (options != null && options.ValidateSignature) {
				var content = await request.ReadAsStringAsync();

				if (!request.IsSignatureValid(content, options))
					throw new ArgumentException("The signature of the webhook is invalid");

				return await WebhookJsonParser.ParseAsync(content, options.JsonSerializerSettings, afterRead, cancellationToken);
			}

			return await request.ReadAsObjectAsync(options?.JsonSerializerSettings, afterRead, cancellationToken);
		}

		private static bool IsSignatureValid(this HttpRequest request, string content, WebhookReceiveOptions options) {
			string signature;
			string algorithm = null;

			switch (options.SignatureLocation) {
				case WebhookSignatureLocation.Header:
					if (!request.Headers.TryGetValue(options.SignatureHeaderName, out var headerValue))
						return false;

					signature = headerValue.SingleOrDefault();

					if (!string.IsNullOrEmpty(signature)) {
						if (signature.StartsWith("sha256=")) {
							signature = signature.Substring("sha256=".Length - 1);
							algorithm = "sha256";
						}
					}

					break;
				case WebhookSignatureLocation.QueryString:
					if (request.Query.Count == 0)
						return false;

					signature = request.Query[options.SignatureQueryStringKey];
					algorithm = request.Query["sig_alg"];

					break;
				default:
					// should never happen
					throw new NotSupportedException();
			}

			if (string.IsNullOrWhiteSpace(signature))
				return false;

			return WebhookSignatureValidator.IsValid(algorithm, content, options.Secret, signature);
		}

	}
}
