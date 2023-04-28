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

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Deveel.Json {
	/// <summary>
	/// A simple JSON converter that can handle polymorphic types.
	/// </summary>
	/// <typeparam name="T">
	/// The base type of the polymorphic hierarchy.
	/// </typeparam>
	/// <remarks>
	/// The types of <typeparamref name="T"/> must be abstract classes
	/// and must define at least one known sub-type using the <see cref="JsonKnownTypeAttribute"/>.
	/// </remarks>
	public sealed class PolymorphicJsonConverter<T> : JsonConverter<T> where T : class {
		private IDictionary<string, Type> knownTypes;

		/// <summary>
		/// Constructs an instance of the converter.
		/// </summary>
		/// <param name="discriminatorProperty">
		/// The name of the property that contains the discriminator value
		/// used to determine the concrete type of the object.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown if the type <typeparamref name="T"/> is not abstract
		/// or if the <paramref name="discriminatorProperty"/> is null or whitespace.
		/// </exception>
		/// <exception cref="JsonException">
		/// Thrown if no known sub-types are defined for the abstract type <typeparamref name="T"/>.
		/// </exception>
		public PolymorphicJsonConverter(string discriminatorProperty) {
			if (!typeof(T).IsAbstract)
				throw new ArgumentException($"The type '{typeof(T)}' is not abstract");

			if (string.IsNullOrWhiteSpace(discriminatorProperty)) 
				throw new ArgumentException($"'{nameof(discriminatorProperty)}' cannot be null or whitespace.", nameof(discriminatorProperty));

			DiscriminatorProperty = discriminatorProperty;

			knownTypes = DiscoverKnownTypes();

			if (knownTypes.Count == 0)
				throw new JsonException($"No known sub-types for the abstract type '{typeof(T)}'");
		}

		/// <summary>
		/// Constructs an instance of the converter, that discovers
		/// the discriminator property name from the <see cref="JsonDiscriminatorAttribute"/>
		/// </summary>
		public  PolymorphicJsonConverter()
			: this(DiscoverDiscriminatorProperty()) {
		}

		/// <summary>
		/// Gets the name of the property that contains the discriminator value
		/// used to determine the concrete type of the object.
		/// </summary>
		public string DiscriminatorProperty { get; }

		private static string DiscoverDiscriminatorProperty() {
			var attr = typeof(T).GetCustomAttribute<JsonDiscriminatorAttribute>();
			return attr?.PropertyName ?? "";
		}

		private IDictionary<string, Type> DiscoverKnownTypes() {
			var result = new Dictionary<string, Type>();
			foreach(var knownType in typeof(T).GetCustomAttributes<JsonKnownTypeAttribute>()) {
				if (result.TryGetValue(knownType.DiscriminatorValue, out var existing))
					throw new JsonException($"The discriminator value '{knownType.DiscriminatorValue}' is already assigned to type '{existing}'");

				if (!typeof(T).IsAssignableFrom(knownType.Type))
					throw new JsonException($"The type '{knownType.Type}' is not assignable from '{typeof(T)}'");

				result[knownType.DiscriminatorValue] = knownType.Type;
			}

			return result;
		}

		/// <inheritdoc/>
		public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			if (!JsonDocument.TryParseValue(ref reader, out var doc))
				throw new JsonException("Invalid JSON input");

			if (!doc.RootElement.TryGetProperty(DiscriminatorProperty, out var property))
				return default(T?);

			if (property.ValueKind != JsonValueKind.String)
				throw new JsonException($"Discriminator property '{DiscriminatorProperty}' is not a string");

			var discriminator = property.GetString();
			if (String.IsNullOrWhiteSpace(discriminator))
				throw new JsonException($"The value of the discriminator property '{DiscriminatorProperty}' is invalid");

			if (!knownTypes.TryGetValue(discriminator, out var type))
				throw new JsonException($"The discriminator value '{discriminator}' was not mapped to any known type");

			return (T?) doc.Deserialize(type, options);
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) {
			if (Equals(value, default(T))) {
				JsonSerializer.Serialize(writer, default(T), typeof(T), options);
			} else {
				var concreteType = value.GetType();
				JsonSerializer.Serialize(writer, value, concreteType, options);
			}
		}
	}
}
