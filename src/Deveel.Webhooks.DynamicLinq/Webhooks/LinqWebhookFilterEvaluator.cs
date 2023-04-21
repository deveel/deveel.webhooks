// Copyright 2022-2023 Deveel
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
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	class LinqWebhookFilterEvaluator<TWebhook> : IWebhookFilterEvaluator<TWebhook> where TWebhook : class {
		public string Format => "linq";

		private Func<TWebhook, bool> Compile(IEnumerable<string> filters) {
			Func<TWebhook, bool> evalFilter = null;
			bool empty = true;

			foreach (var filter in filters) {
				if (WebhookFilter.IsWildcard(filter)) {
					evalFilter = hook => true;
					break;
				} else {
					var config = ParsingConfig.Default;
					var parameters = new[] {
						Expression.Parameter(typeof(TWebhook), "hook")
					};

					var compiled = (Func<TWebhook, bool>)DynamicExpressionParser.ParseLambda(config, parameters, typeof(bool), filter).Compile();
					if (evalFilter == null) {
						evalFilter = compiled;
					} else {
						var prev = (Func<TWebhook, bool>)evalFilter.Clone();
						evalFilter = hook => prev(hook) && compiled(hook);
					}
				}

				empty = false;
			}

			if (empty || evalFilter == null)
				evalFilter = hook => true;

			return evalFilter;
		}

		public Task<bool> MatchesAsync(WebhookSubscriptionFilter request, TWebhook webhook, CancellationToken cancellationToken) {
			if (request is null)
				throw new ArgumentNullException(nameof(request));
			if (webhook is null)
				throw new ArgumentNullException(nameof(webhook));

			if (request.FilterFormat != "linq")
				throw new ArgumentException($"Filter format '{request.FilterFormat}' not supported by the DLINQ evaluator");

			if (request.IsWildcard)
				return Task.FromResult(true);

			var evalFilter = Compile(request.Filters);

			if (evalFilter == null)
				return Task.FromResult(false);

			var result = evalFilter(webhook);

			return Task.FromResult(result);
		}
	}
}
