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

using System.Text.Json.Serialization;

namespace Deveel.Webhooks.Facebook {
    /// <summary>
    /// A single generic element of a product template.
    /// </summary>
    public sealed class ProductElement {
        /// <summary>
        /// The identifier of the element
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the element.
        /// </summary>
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the subtitle of the element.
        /// </summary>
        [JsonPropertyName("subtitle")]
        public string? Subtitle { get; set; }

        /// <summary>
        /// Gets or sets the URL to an image that is displayed 
        /// as part of the element.
        /// </summary>
        [JsonPropertyName("image_url")]
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets a reference to the retailer of 
        /// the product.
        /// </summary>
        [JsonPropertyName("retailer_id")]
        public string? RetailerId { get; set; }
    }
}