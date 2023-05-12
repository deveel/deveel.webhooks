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

// Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618

using Microsoft.Extensions.Primitives;

namespace Deveel.Webhooks.Twilio {
	/// <summary>
	/// Represents a segment of a message received from Twilio.
	/// </summary>
	public sealed class Segment {
		/// <summary>
		/// Gets the zero-based offset of the segment in the message.
		/// </summary>
		public int Index { get; internal set; }

		/// <summary>
		/// Gets the body text of the segment.
		/// </summary>
		public string Text { get; internal set; }

		/// <summary>
		/// Gets the unique identifier of the message part for 
		/// the segment.
		/// </summary>
		public StringValues MessageId { get; internal set; }
	}
}
