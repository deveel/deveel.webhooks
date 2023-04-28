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
    /// Defines the notification frequency of a subscription.
    /// </summary>
    public enum NotificationFequency {
        /// <summary>
        /// Events should be notified daily 
        /// (send 1 notification per 24 hour period 
        /// for 6 months from opt in date)
        /// </summary>
        [EnumMember(Value = "DAILY")]
        Daily,

        /// <summary>
        /// Events should be notified weekly
        /// send 1 notification per week for 9 months 
        /// from the opt in date
        /// </summary>
        [EnumMember(Value = "WEEKLY")]
        Weekly,

        /// <summary>
        /// Events should be notified monthly
        /// (send 1 notification per month for 12 months 
        /// from the opt in date)
        /// </summary>
        [EnumMember(Value = "MONTHLY")]
        Monthly
    }
}
