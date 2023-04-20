using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
