namespace Deveel.Webhooks {
    /// <summary>
    /// Lists all the possible content formats that a webhook 
    /// can be read by a receiver.
    /// </summary>
    [Flags]
    public enum WebhookContentFormats {
        /// <summary>
        /// The receiver cannot support any content format.
        /// </summary>
        None = 0,

        /// <summary>
        /// The receiver can support JSON content format.
        /// </summary>
        Json = 1,

        /// <summary>
        /// The receiver can support XML content format.
        /// </summary>
        Xml = 2,

        /// <summary>
        /// The receiver can support form content format.
        /// </summary>
        Form = 4,

        /// <summary>
        /// The receiver can support all the content formats.
        /// </summary>
        All = Json | Xml | Form
    }
}
