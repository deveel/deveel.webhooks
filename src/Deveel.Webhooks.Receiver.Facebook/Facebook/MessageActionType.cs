namespace Deveel.Facebook {
    /// <summary>
    /// Enumerates the type of actions performed on a message
    /// by a user.
    /// </summary>
    public enum MessageActionType {
        /// <summary>
        /// The user reacted to the message.
        /// </summary>
        React,

        /// <summary>
        /// The user changed the reaction to the message.
        /// </summary>
        Unreact,
    }
}
