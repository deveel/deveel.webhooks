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
    /// Enumerates the possible values for the status of a notification
    /// from an opt-in/opt-out
    /// </summary>
    public enum NotificationStatus {
        /// <summary>
        /// The notifications are stopped.
        /// </summary>
        [EnumMember(Value = "STOP NOTIFICATIONS")]
        Stop,

        /// <summary>
        /// The notifications are resumed.
        /// </summary>
        [EnumMember(Value = "RESUME NOTIFICATIONS")]
        Resume
    }
}
