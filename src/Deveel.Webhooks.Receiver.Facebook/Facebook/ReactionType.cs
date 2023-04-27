namespace Deveel.Facebook {
    /// <summary>
    /// Enumerates the kind of reactions that can be sent
    /// by a user.
    /// </summary>
    public enum ReactionType {
        /// <summary>
        /// The user has liked the message.
        /// </summary>
        Like,

        /// <summary>
        /// The user has disliked the message.
        /// </summary>
        Dislike,

        /// <summary>
        /// The user has reacted with a heart to the message.
        /// </summary>
        Love,

        /// <summary>
        /// The user has reacted with a 'wow' to the message.
        /// </summary>
        Wow,

        /// <summary>
        /// The user has reacted with sadness to the message.
        /// </summary>
        Sad,

        /// <summary>
        /// The user was angry about the message.
        /// </summary>
        Angry,

        /// <summary>
        /// The user has reacted with a smile to the message.
        /// </summary>
        Smile,

        /// <summary>
        /// The user had another reaction to the message.
        /// </summary>
        Other
    }
}
