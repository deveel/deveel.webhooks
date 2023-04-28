namespace Deveel.Webhooks.Twilio {
    public sealed class TwilioWebhook {
        public MessagePart From { get; } = new MessagePart();

        public MessagePart To { get; } = new MessagePart();

        public string SmsMessageSid { get; internal set; }

        public int NumMedia { get; set; }

        public string SmsSid { get; internal set; }

        public SmsStatus SmsStatus { get; internal set; }

        public string Body { get; internal set; }

        public int? NumSegments { get; internal set; }

        public string MessageSid { get; internal set; }

        public string AccountSid { get; internal set; }

        public string ApiVersion { get; internal set; }
    }
}
