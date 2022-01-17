using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Webhooks {
	public sealed class WebhookFilterRequest : IEnumerable<string> {
		private readonly List<string> filters;
		private readonly bool readOnly;

		private WebhookFilterRequest(bool readOnly, string format, IEnumerable<string> filters) {
			if (string.IsNullOrWhiteSpace(format))
				throw new ArgumentException($"'{nameof(format)}' cannot be null or whitespace.", nameof(format));

			this.readOnly = readOnly;

			FilterFormat = format;

			if (filters != null) {
				this.filters = new List<string>(filters);
			} else {
				this.filters = new List<string>();
			}
		}

		public WebhookFilterRequest(string format, IEnumerable<string> filters)
			: this(false, format, filters) {
		}

		public WebhookFilterRequest(string format)
			: this(format, new List<string>()) {
		}

		public string FilterFormat { get; }

		public ICollection<string> Filters => filters.AsReadOnly();

		public bool IsEmpty => filters.Count == 0;

		public bool IsWildcard => filters.Count == 1 && WebhookFilter.IsWildcard(filters[0]);

		public static WebhookFilterRequest Empty => new WebhookFilterRequest(true, "<empty>", null);

		public void AddFilter(string expression) {
			if (readOnly)
				throw new NotSupportedException("The request is read-only");

			lock(filters) {
				filters.Add(expression);
			}
		}

		public IEnumerator<string> GetEnumerator() => filters.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
