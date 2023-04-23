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
using System.Linq.Expressions;

namespace Deveel.Webhooks {
	/// <summary>
	/// Defines a query that can be used to retrieve a page of items
	/// from a pageable store.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public sealed class PagedQuery<TItem> where TItem : class {
		/// <summary>
		/// Constructs the query with the given page number and size.
		/// </summary>
		/// <param name="page">
		/// The number of the page to retrieve, starting from 1.
		/// </param>
		/// <param name="pageSize">
		/// The size of the page to retrieve.
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when the page number is less than 1 or the page size is less than 1.
		/// </exception>
		public PagedQuery(int page, int pageSize) {
			if (page < 1)
				throw new ArgumentOutOfRangeException(nameof(page), "The page must be at least the first");
			if (pageSize < 1)
				throw new ArgumentOutOfRangeException(nameof(pageSize), "The size of a page must be of at least one item");

			Page = page;
			PageSize = pageSize;
		}

		/// <summary>
		/// Gets or sets the predicate that can be used to filter the items
		/// from a store and frame the page.
		/// </summary>
		public Expression<Func<TItem, bool>>? Predicate { get; set; }

		/// <summary>
		/// Gets the number of the page to retrieve, starting from 1.
		/// </summary>
		public int Page { get; }

		/// <summary>
		/// Gets the size of the page to retrieve.
		/// </summary>
		public int PageSize { get; }

		/// <summary>
		/// Gets the zero-based offset within the store of the 
		/// page to retrieve.
		/// </summary>
		public int Offset => (Page - 1) * PageSize;
	}
}
