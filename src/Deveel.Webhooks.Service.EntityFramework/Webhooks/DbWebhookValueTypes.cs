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

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides the types of values that can be
	/// handled by a webhook subscription property.
	/// </summary>
	public static class DbWebhookValueTypes {
		/// <summary>
		/// A string value.
		/// </summary>
		public const string String = "string";

		/// <summary>
		/// A numeric value, that can be either an integer or a floating point.
		/// </summary>
		public const string Number = "number";

		/// <summary>
		/// A boolean value.
		/// </summary>
		public const string Boolean = "bool";

		/// <summary>
		/// An integer number value.
		/// </summary>
		public const string Integer = "integer";

		/// <summary>
		/// A date-time value.
		/// </summary>
		public const string DateTime = "datetime";
	}
}
