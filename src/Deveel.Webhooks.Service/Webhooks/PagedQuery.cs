using System;
using System.Linq.Expressions;

namespace Deveel.Webhooks {
	public sealed class PagedQuery<TItem> where TItem : class {
		public PagedQuery(int page, int pageSize) {
			if (page < 1)
				throw new ArgumentOutOfRangeException(nameof(page), "The page must be at least the first");
			if (pageSize < 1)
				throw new ArgumentOutOfRangeException(nameof(pageSize), "The size of a page must be of at least one item");

			Page = page;
			PageSize = pageSize;
		}

		public Expression<Func<TItem, bool>> Predicate { get; set; }

		public int Page { get; }

		public int PageSize { get; }

		public int Offset => (Page - 1) * PageSize;
	}
}
