// Copyright 2022 Deveel
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

namespace Deveel.Webhooks {
	public class WebhookFilter : IWebhookFilter {
		public const string Wildcard = "*";

		public const string NoFormat = "<empty>";

		public WebhookFilter(string expression, string format) {
			if (string.IsNullOrWhiteSpace(expression))
				throw new ArgumentException($"'{nameof(expression)}' cannot be null or whitespace.", nameof(expression));

			Expression = expression;
			Format = format;
		}

		public string Expression { get; }

		public string Format { get; set; }

		public static WebhookFilter WildcardFilter => new WebhookFilter(Wildcard);

		public static bool IsWildcard(string expression) => String.Equals(expression, Wildcard);
	}
}
