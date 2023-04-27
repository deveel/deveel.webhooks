using System.Text.Json.Serialization;

using Deveel.Json;

namespace Deveel.Facebook {
    public sealed class FacebookWebhookEntry {
		/// <summary>
		/// Constructs a new instance of the entry
		/// </summary>
		/// <param name="id">The unique identifier of the
		/// object associated to the entry</param>
		/// <param name="timeStamp">The time-stamp of the entry</param>
		/// <exception cref="ArgumentException">
		/// Thrown if the given <paramref name="id"/> is <c>null</c> or empty.
		/// </exception>
        [JsonConstructor]
        public FacebookWebhookEntry(string id, DateTimeOffset timeStamp) {
            if (String.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"'{nameof(id)}' cannot be null or whitespace.", nameof(id));

            Id = id;
            TimeStamp = timeStamp;
        }

		/// <summary>
		/// Gets the unique identifier of the object associated 
		/// to the entry.
		/// </summary>
        [JsonPropertyName("id")]
        public string Id { get; }

		/// <summary>
		/// Gets the time-stamp of the entry.
		/// </summary>
        [JsonPropertyName("time")]
        [JsonConverter(typeof(UnixTimeMillisConverter))]
        public DateTimeOffset TimeStamp { get; }

		/// <summary>
		/// Gets a list of messaging entries
		/// </summary>
        [JsonPropertyName("messaging")]
        public IList<MessagingEventEntry>? Messaging { get; set; }
    }
}
