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

using System.Collections;
using System.Reflection;

using MongoDB.Bson;

namespace Deveel.Webhooks {
	static class BsonValueUtil {
		public static BsonDocument ConvertData(object? data) {
			// this is tricky: we try some possible options...

			if (data is null)
				return new BsonDocument();

			IDictionary<string, object?> dictionary;

			if (data is IDictionary<string, object?>) {
				dictionary = (IDictionary<string, object?>)data;
			} else {
				// TODO: make this recursive ...
				dictionary = data.GetType()
					.GetProperties()
					.ToDictionary(x => x.Name, y => y.GetValue(data));
			}

			var document = new BsonDocument();

			// we hope here that the values are supported objects
			foreach (var item in dictionary) {
				document[item.Key] = GetBsonValue(item.Value);
			}

			return document;
		}

		private static BsonDocument GetBsonDocument(object data) {
			var type = data.GetType();
			var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			var doc = new BsonDocument();
			foreach (var property in properties) {
				var value = GetBsonValue(property.GetValue(data));
				doc.Add(property.Name, BsonValue.Create(value));
			}

			return doc;
		}

		private static BsonValue GetBsonValue(object? value) {
			if (value == null)
				return BsonNull.Value;

			if (value is int ||
				value is long ||
				value is double ||
				value is float ||
				value is string ||
				value is DateTime)
				return BsonValue.Create(value);

			if (value is DateTimeOffset dateTimeOffset)
				return new BsonArray(new [] {
					BsonValue.Create(dateTimeOffset.UtcDateTime),
					BsonValue.Create(dateTimeOffset.Offset.Minutes)
				});

			if (value is IEnumerable en)
				return GetBsonArray(en);

			return GetBsonDocument(value);
		}

		private static BsonValue GetBsonArray(IEnumerable en) {
			var array = new BsonArray();

			foreach (var item in en) {
				var value = GetBsonValue(item);
				array.Add(value);
			}

			return array;
		}

	}
}
