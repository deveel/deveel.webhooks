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
