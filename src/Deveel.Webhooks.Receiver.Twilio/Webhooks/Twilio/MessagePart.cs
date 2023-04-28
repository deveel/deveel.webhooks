namespace Deveel.Webhooks.Twilio {
    public sealed record class MessagePart {
        public string PhoneNumber { get; internal set; }

        public string? Country { get; internal set; }

        public string? State { get; internal set; }

        public string? City { get; internal set; }

        public string? Zip { get; internal set; }
    }
}
