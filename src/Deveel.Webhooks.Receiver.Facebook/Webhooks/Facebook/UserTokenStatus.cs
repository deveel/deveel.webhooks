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

using System.Runtime.Serialization;

namespace Deveel.Webhooks.Facebook {
    /// <summary>
    /// Lists the statuses of a user token provided
    /// in an opt-in request.
    /// </summary>
    public enum UserTokenStatus {
        /// <summary>
        /// The user chooses to re opt-in to receiving Recurring Notifications 
        /// after the token has expired
        /// </summary>
        [EnumMember(Value = "REFRESHED")]
        Refreshed,

        /// <summary>
        /// The user does not re opt-in to receiving Recurring Notifications 
        /// after the token has expired (this is the default value).
        /// </summary>
        [EnumMember(Value = "NOT_REFRESHED")]
        NotRefreshed
    }
}
