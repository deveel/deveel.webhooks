namespace Deveel.Webhooks.Twilio {
    /// <summary>
    /// Enumerates the possible values for the status of a Twilio message
    /// </summary>
    public enum MessageStatus {
        /// <summary>
        /// The status of the message is unknown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Twilio has received the message and is attempting to send it
        /// </summary>
        Accepted,

        /// <summary>
        /// Twilio has queued the message for delivery, but has not yet sent it
        /// </summary>
        Queued,

        /// <summary>
        /// Twilio is currently sending the message
        /// </summary>
        Sending,

        /// <summary>
        /// Twilio has successfully sent the message to the destination phone number
        /// </summary>
        Sent,

        /// <summary>
        /// Twilio was unable to send the message to the destination phone number (e.g. due 
        /// to an invalid number).
        /// </summary>
        Failed,

        /// <summary>
        /// The message was successfully delivered to the destination phone number
        /// </summary>
        Delivered,

        /// <summary>
        /// Twilio was unable to deliver the message to the destination phone number
        /// </summary>
        Undelivered,

        /// <summary>
        /// Twilio is currently receiving a reply to the original message
        /// </summary>
        Receiving,

        /// <summary>
        /// Twilio has successfully received a reply to the original message
        /// </summary>
        Received
    }
}
