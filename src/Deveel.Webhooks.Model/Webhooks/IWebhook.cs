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
	/// Notifies the occurrence of an event
	/// and transports related data
	/// </summary>
	public interface IWebhook {
		/// <summary>
		/// Gets an unique identifier of the event
		/// </summary>
		string? Id { get; }

		/// <summary>
		/// Gets the exact time of the event occurrence.
		/// </summary>
		DateTimeOffset TimeStamp { get; }

		/// <summary>
		/// Gets the type of the event
		/// </summary>
		string EventType { get; }

		/// <summary>
		/// Gets the data carried by the webhook to the receiver
		/// </summary>
		object? Data { get; }
	}
}
