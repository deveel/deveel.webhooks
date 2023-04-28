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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Json {
	/// <summary>
	/// Indiates the property that is used as a discriminator
	/// in a polymorphic class.
	/// </summary>
	/// <remarks>
	/// The name of the discriminator property must be the same
	/// as used by the JSON object, and it must be a string.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class JsonDiscriminatorAttribute : Attribute {
		/// <summary>
		/// Constructs the attribute with the given JSON property name.
		/// </summary>
		/// <param name="propertyName">
		/// The name of the JSON property that is used as a discriminator.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown when the given property name is null or whitespace.
		/// </exception>
		public JsonDiscriminatorAttribute(string propertyName) {
			if (string.IsNullOrWhiteSpace(propertyName)) 
				throw new ArgumentException($"'{nameof(propertyName)}' cannot be null or whitespace.", nameof(propertyName));

			PropertyName = propertyName;
		}

		/// <summary>
		/// Gets the name of the JSON property that is used as a discriminator
		/// </summary>
		public string PropertyName { get; }
	}
}
