using System;
using System.Collections.Generic;

namespace Deveel.Webhooks {
	public sealed class PagedResult<TItem> where TItem : class {
		public PagedResult(PagedQuery<TItem> query, int totalCount, IEnumerable<TItem> subscriptions = null) {
			if (totalCount < 0)
				throw new ArgumentOutOfRangeException(nameof(totalCount), "The total count must be equal or greater than zero");

			Query = query ?? throw new ArgumentNullException(nameof(query));
			TotalCount = totalCount;
			Items = subscriptions;
		}

		public PagedQuery<TItem> Query { get; }

		public IEnumerable<TItem> Items { get; set; }

		public int TotalCount { get; }

		public int PageSize => Query.PageSize;

		public int Offset => Query.Offset;

		public int TotalPages => (int)Math.Ceiling((double)TotalCount / Query.PageSize);
	}
}
