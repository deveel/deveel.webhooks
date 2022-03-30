// Copyright 2022 Deveel
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
