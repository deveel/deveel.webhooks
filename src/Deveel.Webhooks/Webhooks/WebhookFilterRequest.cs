using System;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Webhooks {
	public sealed class WebhookFilterRequest {
		private readonly List<WebhookFilterInfo> filters;

		public string FilterProvider { get; }

		public WebhookFilterRequest(string provider) {
			if (string.IsNullOrWhiteSpace(provider)) 
				throw new ArgumentException($"'{nameof(provider)}' cannot be null or whitespace.", nameof(provider));

			FilterProvider = provider;
			filters = new List<WebhookFilterInfo>();
		}

		public IEnumerable<WebhookFilterInfo> Filters => filters.AsReadOnly();

		public bool IsEmpty => filters.Count == 0;

		public bool IsWildcard => filters.Count == 1 && filters[0].IsWildcard;

		public void AddFilter(WebhookFilterInfo filterInfo) {
			lock(filters) {
				filters.Add(filterInfo);
			}
		}

		public void AddFilter(string expression, string format = null)
			=> AddFilter(new WebhookFilterInfo(expression, format));

		public static WebhookFilterRequest FromSubscription(IWebhookSubscription subscription) {
			if (subscription.Filters == null ||
				!subscription.Filters.Any())
				return null;

			var grouped = subscription.Filters.GroupBy(x => x.Provider);
			var providers = grouped.Select(x => x.Key).ToList();

			if (providers.Count > 1)
				throw new NotSupportedException("Multiple filter providers per subscription are not supported yet");

			var request = new WebhookFilterRequest(providers[0]);

			foreach (var group in grouped) {
				if (group.Key == request.FilterProvider) {
					foreach (var filter in group) {
						request.AddFilter(filter.Expression, filter.ExpressionFormat);
					}
				}
			}

			return request;
		}
	}
}
