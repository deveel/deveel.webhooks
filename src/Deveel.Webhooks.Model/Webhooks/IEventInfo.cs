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

namespace Deveel.Webhooks {
	/// <summary>
	/// Defines a contract for an event that might trigger
	/// the notification of a webhook
	/// </summary>
	public interface IEventInfo {
		/// <summary>
		/// Gets the subject of the event (e.g. the name of the aggregate)
		/// </summary>
		string Subject { get; }

		/// <summary>
		/// Gets the type of event that occurred.
		/// </summary>
		string EventType { get; }

		/// <summary>
		/// Gets a unique identifier of the event from the remote system
		/// </summary>
		string Id { get; }

		/// <summary>
		/// Gets the exact time the event occurred
		/// </summary>
		DateTimeOffset TimeStamp { get; }

		/// <summary>
		/// Gets the version of the data carried by the event
		/// </summary>
		string? DataVersion { get; }

		/// <summary>
		/// Gets a set of data carried by the event
		/// </summary>
		object? Data { get; }
	}
}
