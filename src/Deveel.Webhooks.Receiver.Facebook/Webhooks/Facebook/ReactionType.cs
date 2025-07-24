// Copyright 2022-2025 Antonello Provenzano
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Deveel.Webhooks.Facebook
{
    /// <summary>
    /// Enumerates the kind of reactions that can be sent
    /// by a user.
    /// </summary>
    public enum ReactionType
    {
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
