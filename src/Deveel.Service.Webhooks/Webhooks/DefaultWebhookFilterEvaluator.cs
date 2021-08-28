using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Filters;

namespace Deveel.Webhooks {
	class DefaultWebhookFilterEvaluator : IWebhookFilterEvaluator {
		private Func<IWebhook, bool> Compile(IEnumerable<string> filters) {
			Func<IWebhook, bool> evalFilter = null;
			bool empty = true;

			foreach (var filter in filters) {
				if (String.IsNullOrWhiteSpace(filter)) {
					continue;
				} else if (String.Equals("*", filter)) {
					evalFilter = hook => true;
					break;
				} else {
					var compiled = FilterExpression.Compile<IWebhook>("hook", filter);
					if (evalFilter == null) {
						evalFilter = compiled;
					} else {
						var prev = (Func<IWebhook, bool>) evalFilter.Clone();
						evalFilter = hook => prev(hook) && compiled(hook);
					}
				}

				empty = false;
			}

			if (empty)
				evalFilter = hook => true;

			return evalFilter;
		}

		public Task<bool> MatchesAsync(object filter, IWebhook webhook, CancellationToken cancellationToken) {
			if (filter is null) 
				throw new ArgumentNullException(nameof(filter));
			if (webhook is null) 
				throw new ArgumentNullException(nameof(webhook));

			Func<IWebhook, bool> evalFilter = null;

			if (filter is IFilter filterObj) {
				evalFilter = filterObj.Compile<IWebhook>("hook");
			} else if (filter is string s) {
				if (String.IsNullOrWhiteSpace(s)) {
					evalFilter = hook => true;
				} else if (String.Equals(s, "*")) {
					evalFilter = hook => true;
				} else {
					evalFilter = FilterExpression.Compile<IWebhook>("hook", s);
				}
			} else if (filter is IEnumerable<string> filterStrings) {
				evalFilter = Compile(filterStrings);
			} else if (filter is IEnumerable<IFilter> filters) {
				foreach (var f in filters) {
					var compiled = f.Compile<IWebhook>("hook");
					if (evalFilter == null) {
						evalFilter = compiled;
					} else {
						var prev = (Func<IWebhook, bool>)evalFilter.Clone();
						evalFilter = hook => prev(hook) && compiled(hook);
					}
				}
			} else {
				throw new NotSupportedException($"Filter of type {filter.GetType()} is not supported");
			}

			if (evalFilter == null)
				return Task.FromResult(false);

			var result = evalFilter(webhook);

			return Task.FromResult(result);
		}
	}
}
