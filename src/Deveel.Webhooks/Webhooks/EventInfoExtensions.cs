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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides a utility methods to extend the functions of implementations
	/// of the <see cref="IEventInfo"/> contract.
	/// </summary>
	public static class EventInfoExtensions {
		/// <summary>
		/// Creates a new <see cref="EventInfo"/> from the given object
		/// </summary>
		/// <typeparam name="T">
		/// The type of the object that implements the <see cref="IEventInfo"/> contract
		/// </typeparam>
		/// <param name="obj">
		/// The instance of the object that implements the <see cref="IEventInfo"/> contract
		/// </param>
		/// <remarks>
		/// An event is an immutable information across the line of a system that
		/// receives it, and therefore the framework deals only with the <see cref="EventInfo"/>
		/// instances to trigger the notification of a webhook.
		/// </remarks>
		/// <returns>
		/// Returns an instance of <see cref="EventInfo"/> that wraps the given object
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the given <paramref name="obj"/> is <c>null</c>
		/// </exception>
		public static EventInfo AsEventInfo<T>(this T obj)
			where T : IEventInfo {
			if (obj == null)
				throw new ArgumentNullException(nameof(obj));

			return new EventInfo(obj.Subject, obj.EventType, obj.DataVersion, obj.Data, obj.Id, obj.TimeStamp);
		}

		/// <summary>
		/// Attempts to get the value of the given path from the data 
		/// of the event.
		/// </summary>
		/// <typeparam name="TValue">
		/// The type of the value to attempt getting.
		/// </typeparam>
		/// <param name="eventInfo">
		/// The instance of the object that implements the <see cref="IEventInfo"/> contract
		/// </param>
		/// <param name="path">
		/// The path to the value to get from the data of the event.
		/// </param>
		/// <param name="value">
		/// The value of the given path, if found, or the default value of the type
		/// </param>
		/// <returns>
		/// Returns <c>true</c> if the value was found, otherwise <c>false</c>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the given <paramref name="eventInfo"/> is <c>null</c>
		/// </exception>
		/// <exception cref="NotSupportedException">
		/// Thrown if the data of the event is not supported by this reflection.
		/// </exception>
		/// <exception cref="InvalidCastException">
		/// Thrown if the value found in the data of the event cannot be casted
		/// to the given type <typeparamref name="TValue"/>.
		/// </exception>
		public static bool TryGetValue<TValue>(this IEventInfo eventInfo, string path, [MaybeNullWhen(false)] out TValue? value) {
			if (eventInfo == null)
				throw new ArgumentNullException(nameof(eventInfo));

			if (eventInfo.Data == null) {
				value = default;
				return false;
			}

			object? result;

			if (eventInfo.Data is IDictionary<string, JsonElement> jsonData) {
				throw new NotSupportedException("JSON data is not supported yet");
			} else if (eventInfo.Data is IDictionary<string, object> dictionary) {
				if (!dictionary.TryGetValue(path, out result)) {
					value = default;
					return false;
				}
			} else if (!TryGetValueByPath(eventInfo.Data, path, out result))  {
				value = default;
				return false;
			}

			if (!(result is TValue)) {
				result = (TValue?) Convert.ChangeType(result, typeof(TValue), CultureInfo.InvariantCulture);
			}

			value = (TValue?) result;
			return true;
		}

		/// <summary>
		/// Gets the value of the given path from the data of the event.
		/// </summary>
		/// <typeparam name="TValue">
		/// The type of the value to get.
		/// </typeparam>
		/// <param name="eventInfo">
		/// The instance of the object that implements the <see cref="IEventInfo"/> contract
		/// </param>
		/// <param name="path">
		/// The path to the value to get from the data of the event.
		/// </param>
		/// <returns>
		/// Returns the value of the given path, or <c>null</c> if not found.
		/// </returns>
		public static object? GetValue<TValue>(this IEventInfo eventInfo, string path) {
			if (!TryGetValue(eventInfo, path, out object? value))
				return null;

			return value;
		}

		private static bool TryGetValueByPath(object obj, string path, out object? result) {
			var parts = path.Split('.');
			object? value = obj;

			foreach (var part in parts) {
				if (!TryGetMemberValue(value, part, out value)) {
					result = null;
					return false;
				}
			}

			result = value;
			return true;
		}

		private static bool TryGetMemberValue(object? obj, string memberName, out object? value) {
			if (obj == null) {
				value = null;
				return false;
			}

			var type = obj.GetType();

			var member = type.GetMember(memberName).FirstOrDefault();
			if (member == null) {
				value = null;
				return false;
			}

			if (member is PropertyInfo propertyInfo) {
				value = propertyInfo.GetValue(obj);
				return true;
			}

			if (member is FieldInfo fieldInfo) {
				value = fieldInfo.GetValue(obj);
				return true;
			}

			value = null;
			return false;
		}

		private static bool TryGetValueFromJson(JsonElement jsonElement, string path, out object? value) {
			var parts = path.Split('.');

			var current = jsonElement;

			foreach (var part in parts) {
				if (!current.TryGetProperty(part, out current)) {
					value = null;
					return false;
				}
			}

			value = current;
			return true;
		}
	}
}
