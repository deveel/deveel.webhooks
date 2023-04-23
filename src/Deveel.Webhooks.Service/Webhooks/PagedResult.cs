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

using System.Collections.ObjectModel;

namespace Deveel.Webhooks {
	/// <summary>
	/// Represents the result of a paged query, containing the items
	/// in the page and the total count of items in the query.
	/// </summary>
	/// <typeparam name="TItem">
	/// The type of the items in the result.
	/// </typeparam>
	public sealed class PagedResult<TItem> where TItem : class {
		/// <summary>
		/// Initializes a new instance of the <see cref="PagedResult{TItem}"/>
		/// </summary>
		/// <param name="query">
		/// The source query that generated this result.
		/// </param>
		/// <param name="totalCount">
		/// The total count of items in the query.
		/// </param>
		/// <param name="items">
		/// The subset of items in the page.
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		public PagedResult(PagedQuery<TItem> query, int totalCount, IEnumerable<TItem>? items = null) {
			if (totalCount < 0)
				throw new ArgumentOutOfRangeException(nameof(totalCount), "The total count must be equal or greater than zero");

			Query = query ?? throw new ArgumentNullException(nameof(query));
			TotalCount = totalCount;
			Items = items?.ToList().AsReadOnly() ?? new ReadOnlyCollection<TItem>(Array.Empty<TItem>());
		}

		/// <summary>
		/// Gets the source query that generated this result.
		/// </summary>
		public PagedQuery<TItem> Query { get; }

		/// <summary>
		/// Gets the subset of items in the page.
		/// </summary>
		public IReadOnlyList<TItem> Items { get; set; }

		/// <summary>
		/// Gets the total count of items in the query.
		/// </summary>
		public int TotalCount { get; }

		/// <summary>
		/// Gets the size pf the page, as specified in the query.
		/// </summary>
		public int PageSize => Query.PageSize;

		/// <summary>
		/// Gets the offset within the source storage where the page starts.
		/// </summary>
		public int Offset => Query.Offset;

		/// <summary>
		/// Counts the total number of pages in the query.
		/// </summary>
		public int TotalPages => (int)Math.Ceiling((double)TotalCount / Query.PageSize);
	}
}
