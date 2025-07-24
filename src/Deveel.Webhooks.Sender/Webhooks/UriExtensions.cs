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
	/// Extends the <see cref="Uri"/> class with additional
	/// methods for the building of URLs
	/// </summary>
	public static class UriExtensions {
		/// <summary>
		/// Adds or set a query parameter to the given <paramref name="uri"/>
		/// </summary>
		/// <param name="uri">
		/// The URI to which the query parameter is added
		/// </param>
		/// <param name="name">
		/// The name of the query parameter
		/// </param>
		/// <param name="value">
		/// The value of the query parameter
		/// </param>
		/// <returns>
		/// Returns a new instance of <see cref="Uri"/> with the query
		/// parameter set.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown when the <paramref name="name"/> is null or empty
		/// </exception>
		public static Uri AddQueryParameter(this Uri uri, string name, string? value) {
			if (string.IsNullOrWhiteSpace(name)) 
				throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));

			var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
			query[name] = value;

			var builder = new UriBuilder(uri) {
				Query = query.ToString()
			};

			return builder.Uri;
		}
	}
}
