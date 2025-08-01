﻿// Copyright 2022-2025 Antonello Provenzano
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
	/// Provides the options for handling a Facebook webhook.
	/// </summary>
	public sealed class FacebookWebhookHandlingOptions {
		/// <summary>
		/// Gets or sets the maximum number of parallel threads to use
		/// while receiving and handling webhooks (defaults to the number
		/// processors available in the system, minus one).
		/// </summary>
		public int? MaxParallelThreads { get; set; } = Environment.ProcessorCount - 1;

		/// <summary>
		/// Gets or sets the execution mode of the handlers
		/// (defaults to <see cref="HandlerExecutionMode.Parallel"/>).
		/// </summary>
		public HandlerExecutionMode ExecutionMode { get; set; } = HandlerExecutionMode.Parallel;
	}
}
