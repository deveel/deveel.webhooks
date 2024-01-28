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

using System;

namespace Deveel.Webhooks {
	/// <summary>
	/// A default implementation of <see cref="IWebhookFilter"/> that can be used
	/// to represent a filter expression.
	/// </summary>
	public readonly struct WebhookFilter : IWebhookFilter, IEquatable<IWebhookFilter> {
		/// <summary>
		/// Constructs the filter with the given expression and format.
		/// </summary>
		/// <param name="expression">
		/// The string expression that can be used to match a webhook.
		/// </param>
		/// <param name="format">
		/// The format of the expression.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown when the given <paramref name="expression"/> or <paramref name="format"/> 
		/// are <c>null</c> or empty.
		/// </exception>
		public WebhookFilter(string expression, string format) {
			if (string.IsNullOrWhiteSpace(expression))
				throw new ArgumentException($"'{nameof(expression)}' cannot be null or whitespace.", nameof(expression));

			if (string.IsNullOrWhiteSpace(format))
				throw new ArgumentException($"'{nameof(format)}' cannot be null or whitespace.", nameof(format));

			Expression = expression;
			Format = format;
		}

		/// <summary>
		/// A wildcard expression that can be used to match any webhook.
		/// </summary>
		public const string Wildcard = "*";

		/// <summary>
		/// A special name that can be used to represent a filter
		/// that has no format.
		/// </summary>
		public const string NoFormat = "<empty>";

		/// <summary>
		/// Gets the string expression that can be used to match a webhook.
		/// </summary>
		public string Expression { get; }

		/// <summary>
		/// Gets the format of the expression.
		/// </summary>
		public string Format { get; }

		/// <summary>
		/// Gets a <see cref="IWebhookFilter"/> that matches any webhook.
		/// </summary>
		public static WebhookFilter WildcardFilter => new WebhookFilter(Wildcard, NoFormat);

		/// <summary>
		/// Checks if the given <paramref name="expression"/> is a wildcard.
		/// </summary>
		/// <param name="expression">
		/// The string expression to check.
		/// </param>
		/// <returns>
		/// Returns <c>true</c> if the given <paramref name="expression"/> is a wildcard,
		/// otherwise <c>false</c>.
		/// </returns>
		public static bool IsWildcard(string expression) => String.Equals(expression, Wildcard);

		/// <inheritdoc/>
		public override bool Equals(object? obj) => obj is IWebhookFilter filter && Equals(filter);

		/// <inheritdoc/>
		public bool Equals(IWebhookFilter? other) => other != null &&
			((this.IsWildcard() && other.IsWildcard()) || 
			(Expression == other.Expression && Format == other.Format));

		/// <inheritdoc/>
		public override int GetHashCode() => this.IsWildcard() ? HashCode.Combine(Wildcard) : HashCode.Combine(Expression, Format);
	}
}
