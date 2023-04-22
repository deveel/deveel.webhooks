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

using System.Text;
using System.Text.Json;

namespace Deveel.Webhooks {
	/// <summary>
	/// Extends the <see cref="IWebhookJsonSerializer{TWebhook}"/> interface
	/// to provide methods to serialize webhoosk.
	/// </summary>
	public static class WebhookJsonSerializerExtensions {
        /// <summary>
        /// Serializes the given webhook to a string.
        /// </summary>
        /// <typeparam name="TWebhook">
        /// The type of the webhook to serialize.
        /// </typeparam>
        /// <param name="serializer">
        /// The instance of the serializer to use.
        /// </param>
        /// <param name="webhook">
        /// The instance of the webhook to serialize.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token to cancel the operation.
        /// </param>
        /// <returns>
        /// Returns a JSON-formatted string containing the serialized webhook.
        /// </returns>
        /// <exception cref="WebhookSerializationException">
        /// Thrown if the webhook cannot be serialized.
        /// </exception>
        public static async Task<string> SerializeToStringAsync<TWebhook>(this IWebhookJsonSerializer<TWebhook> serializer, TWebhook webhook, CancellationToken cancellationToken = default)
            where TWebhook : class {

            try {
                using var stream = new MemoryStream();
                await serializer.SerializeAsync(stream, webhook, cancellationToken);
                stream.Position = 0;

                using var reader = new StreamReader(stream, Encoding.UTF8);
                return await reader.ReadToEndAsync();
            } catch (WebhookSerializationException) {
                throw;
            } catch (Exception ex) {
                throw new WebhookSerializationException("Error while serializing the webhook", ex);
            }
        }

		/// <summary>
		/// Serializes the given webhook to an anonymous object.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of the webhook to serialize.
		/// </typeparam>
		/// <param name="serializer">
		/// The instance of the serializer to use.
		/// </param>
		/// <param name="webhook">
		/// The instance of the webhook to serialize.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns an anonymous object containing the serialized webhook.
		/// </returns>
		public static async Task<object?> SerializeToObjectAsync<TWebhook>(this IWebhookJsonSerializer<TWebhook> serializer, TWebhook webhook, CancellationToken cancellationToken = default)
			where TWebhook : class {

			try {
				Func<TWebhook, CancellationToken, Task<object?>> serialize = async (w, ct) => {
					using var stream = new MemoryStream();
					await serializer.SerializeAsync(stream, webhook, cancellationToken);
					stream.Position = 0;

					var jsonElement = await JsonSerializer.DeserializeAsync<JsonElement>(stream, cancellationToken: cancellationToken);

					return ToAnonymousType(jsonElement);
				};

				return await WebhookTypeCache.Default.GetOrCreateAsync(webhook, serialize, cancellationToken);
			} catch (Exception ex) {
				throw new WebhookSerializationException("It was not possible to serialize the webhook", ex);
			}
		}

		private static object? ToAnonymousType(JsonElement element) {
			if (element.ValueKind == JsonValueKind.Object) {
				var anonymousObject = new Dictionary<string, object?>();
				foreach (var property in element.EnumerateObject()) {
					anonymousObject[property.Name] = ToAnonymousType(property.Value);
				}

				return TypeCreator.CreateAnonymousType(anonymousObject);
			} else if (element.ValueKind == JsonValueKind.Array) {
				var array = new List<object?>();
				foreach (var item in element.EnumerateArray()) {
					array.Add(ToAnonymousType(item));
				}
				return array.ToArray();
			} else {
				return GetJsonValue(element);
			}
		}

		private static object? GetJsonValue(JsonElement element) {
			switch (element.ValueKind) {
				case JsonValueKind.String:
					if (element.TryGetGuid(out var guid))
						return guid;

					if (element.TryGetDateTimeOffset(out var date2))
						return date2;
					if (element.TryGetDateTime(out var date))
						return date;

					return element.GetString();
				case JsonValueKind.Number:
					if (element.TryGetInt32(out var i))
						return i;
					if (element.TryGetInt64(out var l))
						return l;

					return element.GetDouble();
				case JsonValueKind.True:
					return true;
				case JsonValueKind.False:
					return false;
				case JsonValueKind.Null:
					return null;
				default:
					throw new ArgumentException($"Invalid JsonElement value kind '{element.ValueKind}'.");
			}
		}
	}
}
