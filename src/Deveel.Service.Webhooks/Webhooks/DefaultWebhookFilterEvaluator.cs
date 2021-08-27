using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Filters;

namespace Deveel.Webhooks {
	class DefaultWebhookFilterEvaluator : IWebhookFilterEvaluator {
		public Task<bool> MatchesAsync(object filter, IWebhook webhook, CancellationToken cancellationToken) {
			if (filter == null)
				return Task.FromResult(true);
			if (webhook == null)
				return Task.FromResult(false);

			Func<IWebhook, bool> evalFilter = null;

			if (filter is IFilter filterObj) {
				evalFilter = filterObj.Compile<IWebhook>("hook");
			} else if (filter is string s) {
				if (String.Equals(s, "*")) {
					evalFilter = hook => true;
				} else {
					evalFilter = FilterExpression.Compile<IWebhook>("hook", s);
				}
			} else if (filter is IEnumerable<string> filterStrings) {
				foreach (var s1 in filterStrings) {
					if (String.Equals("*", s1)) {
						evalFilter = hook => true;
						break;
					} else {
						var compiled = FilterExpression.Compile<IWebhook>("hook", s1);
						if (evalFilter == null) {
							evalFilter = compiled;
						} else {
							evalFilter = hook => evalFilter(hook) && compiled(hook);
						}
					}
				}
			} else if (filter is IEnumerable<IFilter> filters) {
				foreach (var f in filters) {
					var compiled = f.Compile<IWebhook>("hook");
					if (evalFilter == null) {
						evalFilter = compiled;
					} else {
						evalFilter = hook => evalFilter(hook) && compiled(hook);
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
