using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Webhooks {
	public sealed class WebhookFilterRequest : IEnumerable<IWebhookFilter> {
		private readonly List<IWebhookFilter> filters;
		private readonly bool readOnly;

		private WebhookFilterRequest(bool readOnly, IEnumerable<IWebhookFilter> filters) {
			this.readOnly = readOnly;

			if (filters != null) {
				this.filters = new List<IWebhookFilter>(filters);
			} else {
				this.filters = new List<IWebhookFilter>();
			}
		}

		public WebhookFilterRequest(IEnumerable<IWebhookFilter> filters)
			: this(false, filters) {
		}

		public WebhookFilterRequest()
			: this(new List<IWebhookFilter>()) {
		}

		public ICollection<IWebhookFilter> Filters => filters.AsReadOnly();

		public bool IsEmpty => filters.Count == 0;

		public bool IsWildcard => filters.Count == 1 && filters[0].IsWildcard();

		public static WebhookFilterRequest Empty => new WebhookFilterRequest(true, null);

		public void AddFilter(IWebhookFilter filterInfo) {
			if (readOnly)
				throw new NotSupportedException("The request is read-only");

			lock(filters) {
				filters.Add(filterInfo);
			}
		}

		public void AddFilter(string expression, string format = null)
			=> AddFilter(new WebhookFilter(expression, format));

		public IEnumerator<IWebhookFilter> GetEnumerator() => filters.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
