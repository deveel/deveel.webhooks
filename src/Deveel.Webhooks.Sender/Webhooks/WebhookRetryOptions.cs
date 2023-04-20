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
	/// Provides options for the retry policy of the webhook sender.
	/// </summary>
    public class WebhookRetryOptions {
		/// <summary>
		/// Initializes a new instance of the <see cref="WebhookRetryOptions"/> class.
		/// </summary>
		public WebhookRetryOptions() {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WebhookRetryOptions"/> class
		/// copying the options from the given instance.
		/// </summary>
		/// <param name="options"></param>
		public WebhookRetryOptions(WebhookRetryOptions options) {
			MaxRetries = options.MaxRetries;
			MaxDelay = options.MaxDelay;
			Timeout = options.Timeout;
		}

		/// <summary>
		/// Gets or sets the maximum number of retries,
		/// after the first attempt. If this is not set,
		/// no retries are performed.
		/// </summary>
		public int? MaxRetries { get; set; }

		/// <summary>
		/// Gets or sets the maximum delay between retries
		/// (500 milliseconds by default).
		/// </summary>
		public TimeSpan? MaxDelay { get; set; } = TimeSpan.FromMilliseconds(500);

		/// <summary>
		/// Gets or sets a timeout for the single attempt. When
		/// this is not set, the infinite timeout is used.
		/// </summary>
		public TimeSpan? Timeout { get; set; }

		/// <summary>
		/// Merges the options of this instance with the given instance
		/// </summary>
		/// <param name="options">
		/// The options to merge with this instance.
		/// </param>
		/// <returns>
		/// Returns a new instance of <see cref="WebhookRetryOptions"/> that
		/// is the result of the merge between this instance and the given
		/// options.
		/// </returns>
		public WebhookRetryOptions Merge(WebhookRetryOptions options) {
			return new WebhookRetryOptions {
				MaxRetries = MaxRetries ?? options.MaxRetries,
				MaxDelay = MaxDelay ?? options.MaxDelay,
				Timeout = Timeout ?? options.Timeout
			};
		}
	}
}
