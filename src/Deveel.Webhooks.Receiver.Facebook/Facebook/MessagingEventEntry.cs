using System;
using System.Text.Json.Serialization;

using Deveel.Json;

namespace Deveel.Facebook {
    public sealed class MessagingEventEntry {
        [JsonConstructor]
        public MessagingEventEntry(MessagingPart sender, MessagingPart recipient) {
            Sender = sender;
            Recipient = recipient;
        }

        [JsonPropertyName("sender")]
        public MessagingPart Sender { get; }

        [JsonPropertyName("recipient")]
        public MessagingPart Recipient { get; }

        [JsonPropertyName("delivery")]
        public MessageDelivery? Delivery { get; set; }

        [JsonPropertyName("read")]
        public MessageRead? Read { get; set; }

		[JsonPropertyName("message")]
		public Message? Message { get; set; }
    }
}
