// Copyright 2022-2024 Antonello Provenzano
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

using System;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides configuration options for the notification of webhooks.
	/// </summary>
	/// <typeparam name="TWebhook"></typeparam>
	public class WebhookNotificationOptions<TWebhook> {
		/// <summary>
		/// Gets or sets the number of parallel threads that will be used
		/// to send the notifications.
		/// </summary>
		public int ParallelThreadCount { get; set; } = Environment.ProcessorCount - 1;
	}
}
