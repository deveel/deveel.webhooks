using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	/// <summary>
	/// A utility class that can be used to serialize and deserialize
	/// Facebook entities to and from JSON
	/// </summary>
	public static class FacebookJsonSerializer {
		static FacebookJsonSerializer() {
			var serializerOptions = new JsonSerializerOptions {
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
				PropertyNameCaseInsensitive = true,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};

			serializerOptions.Converters.Add(new JsonStringEnumMemberConverter(JsonNamingPolicy.CamelCase));

			Options = serializerOptions;
		}

		/// <summary>
		/// Gets the serialization options
		/// </summary>
		public static JsonSerializerOptions Options { get; }

		/// <summary>
		/// Deserializes a given UTF-8 stream to a Facebook Messenger webhook
		/// </summary>
		/// <param name="utf8Stream">The UTF-8 to be read for deserialization</param>
		/// <returns>
		/// Returns an instance of <see cref="FacebookWebhook"/> that is the result
		/// of the deserialization.
		/// </returns>
		public static FacebookWebhook? DeserializeWebhook(Stream utf8Stream)
			=> JsonSerializer.Deserialize<FacebookWebhook>(utf8Stream, Options);

		/// <summary>
		/// Deserializes a given UTF-8 stream to a Facebook webhook
		/// </summary>
		/// <param name="utf8Stream">The UTF-8 to be read for deserialization</param>
		/// <param name="cancellationToken"></param>
		/// <returns>
		/// Returns an instance of <see cref="FacebookWebhook"/> that is the result
		/// of the deserialization.
		/// </returns>
		public static async Task<FacebookWebhook?> DeserializeWebhookAsync(Stream utf8Stream, CancellationToken cancellationToken = default)
			=> await JsonSerializer.DeserializeAsync<FacebookWebhook>(utf8Stream, Options, cancellationToken);

		/// <summary>
		/// Deserializes a given JSON-formatted string to a Facebook webhook
		/// </summary>
		/// <param name="json">The JSON-formatted string to read for deserialization</param>
		/// <returns>
		/// Returns an instance of <see cref="FacebookWebhook"/> that is the result
		/// of the deserialization.
		/// </returns>
		public static FacebookWebhook? DeserializeWebhook(string json) {
			return JsonSerializer.Deserialize<FacebookWebhook>(json, Options);
		}

		public static string SerializeWebhook(FacebookWebhook? webhook)
			=> JsonSerializer.Serialize(webhook, Options);

		public static async Task<string> SerializeWebhookAsync(FacebookWebhook? webhook, CancellationToken cancellationToken = default) {
			using var stream = new MemoryStream();
			await JsonSerializer.SerializeAsync(stream, webhook, Options, cancellationToken);

			await stream.FlushAsync();
			stream.Seek(0, SeekOrigin.Begin);

			return Encoding.UTF8.GetString(stream.ToArray());
		}

	}
}
