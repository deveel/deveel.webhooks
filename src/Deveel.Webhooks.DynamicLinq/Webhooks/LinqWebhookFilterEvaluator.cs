// Copyright 2022-2024 Antonello Provenzano
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

using System.Collections.Concurrent;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	/// <summary>
	/// A service that evaluates a webhook filter using a LINQ expression
	/// </summary>
	/// <typeparam name="TWebhook"></typeparam>
	public sealed class LinqWebhookFilterEvaluator<TWebhook> : IWebhookFilterEvaluator<TWebhook> where TWebhook : class {
		private readonly IDictionary<FilterKey, Func<object, bool>> filterCache;
		private readonly WebhookSenderOptions<TWebhook> senderOptions;

		/// <summary>
		/// Constructs the evaluator with the given options
		/// from the sender.
		/// </summary>
		/// <param name="senderOptions">
		/// The options from the sender that are used to configure
		/// the filter evaluator.
		/// </param>
		public LinqWebhookFilterEvaluator(IOptions<WebhookSenderOptions<TWebhook>> senderOptions) {
			filterCache = new ConcurrentDictionary<FilterKey, Func<object, bool>>();
			this.senderOptions = senderOptions.Value;
		}

		static LinqWebhookFilterEvaluator() {
			Default = new LinqWebhookFilterEvaluator<TWebhook>(Options.Create(new WebhookSenderOptions<TWebhook>()));
		}

		/// <summary>
		/// Gets a default instance of the evaluator.
		/// </summary>
		public static LinqWebhookFilterEvaluator<TWebhook> Default { get; }

		string IWebhookFilterEvaluator<TWebhook>.Format => "linq";

		private Func<object, bool> Compile(Type objType, string filter) {
			var key = new FilterKey(objType.FullName!, filter);
			if (!filterCache.TryGetValue(key, out var compiled)) {
				var config = new ParsingConfig{
					// Use the default options for parsing
					// LINQ expressions
					IsCaseSensitive = false,
					AllowEqualsAndToStringMethodsOnObject = true,
				};

				var parameters = new[] {
					Expression.Parameter(objType, "hook")
				};
				var parsed = DynamicExpressionParser.ParseLambda(config, parameters, typeof(bool), filter).Compile();
				compiled = hook => (bool)(parsed.DynamicInvoke(hook)!);
				filterCache[key] = compiled;
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

		/// <inheritdoc/>
		public async Task<bool> MatchesAsync(WebhookSubscriptionFilter filter, TWebhook webhook, CancellationToken cancellationToken) {
			ArgumentNullException.ThrowIfNull(filter, nameof(filter));
			ArgumentNullException.ThrowIfNull(webhook, nameof(webhook));

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

		readonly struct FilterKey {
			public FilterKey(string typeName, string filter) : this() {
				TypeName = typeName;
				Filter = filter;
			}

			public string TypeName { get; }

			public string Filter { get; }

			public override bool Equals(object? obj) {
				return obj is FilterKey key &&
					   TypeName == key.TypeName &&
					   Filter == key.Filter;
			}

			public override int GetHashCode() {
				return HashCode.Combine(TypeName, Filter);
			}
		}
	}
}
