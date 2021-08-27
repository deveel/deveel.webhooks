using System;
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

			Func<IWebhook, bool> evalFilter;

			if (filter is IFilter filterObj) {
				evalFilter = filterObj.Compile<IWebhook>("hook");
			} else if (filter is string s) {
				evalFilter = FilterExpression.Compile<IWebhook>("hook", s);
			} else {
				throw new NotSupportedException($"Filter of type {filter.GetType()} is not supported");
			}

			var result = evalFilter(webhook);

			return Task.FromResult(result);
		}
	}
}
