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

using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	public sealed class LinqWebhookFilterEvaluator<TWebhook> : IWebhookFilterEvaluator<TWebhook> where TWebhook : class {
		private readonly IDictionary<string, Func<object, bool>> filterCache;
		private readonly WebhookSenderOptions<TWebhook> senderOptions;

		public LinqWebhookFilterEvaluator(IOptions<WebhookSenderOptions<TWebhook>> senderOptions) {
			filterCache = new Dictionary<string, Func<object, bool>>();
			this.senderOptions = senderOptions.Value;
		}

		static LinqWebhookFilterEvaluator() {
			Default = new LinqWebhookFilterEvaluator<TWebhook>(Options.Create(new WebhookSenderOptions<TWebhook>()));
		}

		public static LinqWebhookFilterEvaluator<TWebhook> Default { get; }

		string IWebhookFilterEvaluator<TWebhook>.Format => "linq";

		private Func<object, bool> Compile(Type objType, string filter) {
			if (!filterCache.TryGetValue(filter, out var compiled)) {
				var config = ParsingConfig.Default;

				var parameters = new[] {
					Expression.Parameter(objType, "hook")
				};
				var parsed = DynamicExpressionParser.ParseLambda(config, parameters, typeof(bool), filter).Compile();
				compiled = hook => (bool)(parsed.DynamicInvoke(hook)!);
				filterCache[filter] = compiled;
			}

			return compiled;
		}

		private Func<object, bool> Compile(Type objType, IList<string> filters) {
			var hasWildcard = filters.Any(WebhookFilter.IsWildcard);
			if (hasWildcard)
				return hook => true;

			var exp = String.Join(" && ", filters);
			return Compile(objType, exp);
		}

		private Task<object?> SerializeToObjectAsync(TWebhook webhook, CancellationToken cancellationToken) {
			// TODO: check the format and select the proper serializer
			if (senderOptions.JsonSerializer == null)
				throw new NotSupportedException("Serialization is not supported by the sender");

			return senderOptions.JsonSerializer.SerializeToObjectAsync(webhook, cancellationToken);
		}

		public async Task<bool> MatchesAsync(WebhookSubscriptionFilter filter, TWebhook webhook, CancellationToken cancellationToken) {
			if (filter is null)
				throw new ArgumentNullException(nameof(filter));
			if (webhook is null)
				throw new ArgumentNullException(nameof(webhook));

			if (filter.FilterFormat != "linq")
				throw new ArgumentException($"Filter format '{filter.FilterFormat}' not supported by the LINQ evaluator");

			if (filter.IsWildcard)
				return true;

			try {
				var obj = await SerializeToObjectAsync(webhook, cancellationToken);

				if (obj is null)
					return false;

				var evalFilter = Compile(obj.GetType(), filter.Filters?.ToList() ?? new List<string>());

				var result = evalFilter(obj);

				return result;
			} catch (WebhookSerializationException ex) {
				throw new WebhookException("The webhook object is invalid", ex);
			} catch(Exception ex) {
				throw new WebhookException("Unable to evaluate the filter", ex);
			}

		}
	}
}
