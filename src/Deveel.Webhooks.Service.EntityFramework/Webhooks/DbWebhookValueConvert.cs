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

using System.Globalization;

namespace Deveel.Webhooks {
	/// <summary>
	/// A class that provides methods to convert values to and from
	/// a string representation that can be stored in a database.
	/// </summary>
	public static class DbWebhookValueConvert {
		/// <summary>
		/// Gets the type of the value passed as argument.
		/// </summary>
		/// <param name="value">
		/// The object value to get the type of.
		/// </param>
		/// <returns>
		/// Returns a string that represents the type of the value,
		/// as one of the constants defined in <see cref="DbWebhookValueTypes"/>.
		/// </returns>
		/// <exception cref="NotSupportedException">
		/// Thrown when the type of the value is not supported.
		/// </exception>
		public static string GetValueType(object? value) {
			if (value is null || value is string)
				return DbWebhookValueTypes.String;
			if (value is int || value is long)
				return DbWebhookValueTypes.Integer;
			if (value is bool)
				return DbWebhookValueTypes.Boolean;
			if (value is double || value is float)
				return DbWebhookValueTypes.Number;
			if (value is DateTime)
				return DbWebhookValueTypes.DateTime;

			throw new NotSupportedException($"The value of type '{value.GetType()}' is not supported");
		}

		/// <summary>
		/// Converts the given value to a string representation that can be stored
		/// in the database.
		/// </summary>
		/// <param name="value">
		/// The value object to convert.
		/// </param>
		/// <returns>
		/// Returns a string that represents the value, or <c>null</c> if the value
		/// is <c>null</c>.
		/// </returns>
		/// <exception cref="NotSupportedException">
		/// Thrown when the type of the value is not supported.
		/// </exception>
		public static string? ConvertToString(object? value) {
			if (value is null)
				return null;
			if (value is bool b)
				return b ? "true" : "false";
			if (value is string str)
				return str;
			if (value is int || value is long)
				return System.Convert.ToString(value, CultureInfo.InvariantCulture);
			if (value is double || value is float)
				return System.Convert.ToString(value, CultureInfo.InvariantCulture);
			if (value is DateTime dt)
				return dt.ToString("O");

			throw new NotSupportedException($"The value of type '{value.GetType()}' is not supported");
		}

		/// <summary>
		/// Converts the given string value to the given type.
		/// </summary>
		/// <param name="value">
		/// The string value to convert.
		/// </param>
		/// <param name="valueType">
		/// The type of the value to convert to, as one of the constants
		/// defined in <see cref="DbWebhookValueTypes"/>.
		/// </param>
		/// <returns>
		/// Returns an object that represents the converted value, or <c>null</c>
		/// if the given string value is <c>null</c>.
		/// </returns>
		/// <exception cref="NotSupportedException">
		/// Thrown when the type of the value is not supported.
		/// </exception>
		public static object? Convert(string? value, string valueType) {
			if (value is null)
				return null;

			return valueType switch {
				DbWebhookValueTypes.Boolean => ParseBoolean(value),
				var x when x == DbWebhookValueTypes.Integer && Int32.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i32) => i32,
				var x when x == DbWebhookValueTypes.Integer && Int64.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i64) => i64,
				var x when x == DbWebhookValueTypes.Number && Double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) => d,
				var x when x == DbWebhookValueTypes.Number && Single.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var f) => f,
				DbWebhookValueTypes.String => value,
				DbWebhookValueTypes.DateTime => DateTime.Parse(value, CultureInfo.InvariantCulture),
				_ => throw new NotSupportedException($"The value type '{valueType}' is not supported")
			};
		}

		private static bool ParseBoolean(string value) {
			return value switch {
				"true" => true,
				"false" => false,
				_ => Boolean.Parse(value)
			};
		}
	}
}
