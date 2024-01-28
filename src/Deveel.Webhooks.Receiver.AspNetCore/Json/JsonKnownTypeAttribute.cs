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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Json {
	/// <summary>
	/// Defines the type of object that is used by the
	/// converter when deserializing a polymorphic class
	/// from a JSON object.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class JsonKnownTypeAttribute : Attribute {
		/// <summary>
		/// Constructs the attribute with the given type
		/// and discriminator value.
		/// </summary>
		/// <param name="type">
		/// The type of the object that is used by the JSON converter
		/// then the discriminator value is matched.
		/// </param>
		/// <param name="discriminatorValue">
		/// The value of the discriminator property that is used
		/// to match the type of the object that the converter
		/// will deserialize.
		/// </param>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		public JsonKnownTypeAttribute(Type type, string discriminatorValue) {
			if (string.IsNullOrWhiteSpace(discriminatorValue)) 
				throw new ArgumentException($"'{nameof(discriminatorValue)}' cannot be null or whitespace.", nameof(discriminatorValue));

			Type = type ?? throw new ArgumentNullException(nameof(type));
			DiscriminatorValue = discriminatorValue;
		}

		/// <summary>
		/// Gets the type of the object that is used by the JSON converter
		/// to deserialize the object when the discriminator value is matched.
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// Gets the value of the discriminator property that is used
		/// to match the type of the object that the converter
		/// will deserialize.
		/// </summary>
		public string DiscriminatorValue { get; }
	}
}
