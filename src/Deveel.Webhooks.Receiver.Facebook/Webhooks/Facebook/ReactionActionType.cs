// Copyright 2022-2023 Deveel
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

namespace Deveel.Webhooks.Facebook {
    /// <summary>
    /// Enumerates the type of actions performed on
    /// a message by a user.
    /// </summary>
    public enum ReactionActionType {
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
