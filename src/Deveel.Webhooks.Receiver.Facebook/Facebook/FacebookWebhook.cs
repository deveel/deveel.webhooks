using System;
using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	/// <summary>
	/// A webhook sent from the Facebook Messenger Platform
	/// </summary>
    public sealed class FacebookWebhook {
		/// <summary>
		/// Constructs a new instance of the webhook object
		/// </summary>
		/// <param name="object">The type of object the webhook
		/// relates to.</param>
        [JsonConstructor]
        public FacebookWebhook(string @object) {
            Object = @object;
        }

        // TODO: use an enumeration
		/// <summary>
		/// Gets the type of object this webhook relates to.
		/// </summary>
        [JsonPropertyName("object")]
        public string Object { get; }

		/// <summary>
		/// Gets an array of entries
		/// </summary>
        [JsonPropertyName("entry")]
        public FacebookWebhookEntry[]? Entries { get; set; }
    }
}
