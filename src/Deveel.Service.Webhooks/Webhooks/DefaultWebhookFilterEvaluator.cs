using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	class DefaultWebhookFilterEvaluator : IWebhookFilterEvaluator {
		private Func<IWebhook, bool> Compile(IEnumerable<IWebhookFilter> filters) {
			Func<IWebhook, bool> evalFilter = null;
			bool empty = true;

			foreach (var filter in filters) {
				if (filter.IsWildcard()) {
					evalFilter = hook => true;
					break;
				} else {
					// TODO: argument name?
					var config = new ParsingConfig {
					};

					var compiled = DynamicExpressionParser.ParseLambda<IWebhook, bool>(config, false, filter.Expression).Compile();
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

		public Task<bool> MatchesAsync(WebhookFilterRequest request, IWebhook webhook, CancellationToken cancellationToken) {
			if (request is null) 
				throw new ArgumentNullException(nameof(request));
			if (webhook is null) 
				throw new ArgumentNullException(nameof(webhook));

			var evalFilter = Compile(request.Filters);

			if (evalFilter == null)
				return Task.FromResult(false);

			var result = evalFilter(webhook);

			return Task.FromResult(result);
		}
	}
}
