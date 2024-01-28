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
using System.Collections;
using System.Collections.Generic;

namespace Deveel.Webhooks {
	/// <summary>
	/// A filter that can be used to match the webhooks to notify
	/// to a given subscription.
	/// </summary>
	public sealed class WebhookSubscriptionFilter {
		private readonly List<string> filters;
		private readonly bool readOnly;

		private WebhookSubscriptionFilter(bool readOnly, string format, IEnumerable<string>? filters) {
			if (string.IsNullOrWhiteSpace(format))
				throw new ArgumentException($"'{nameof(format)}' cannot be null or whitespace.", nameof(format));

			this.readOnly = readOnly;

			FilterFormat = format;

			this.filters = filters?.ToList() ?? new List<string>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WebhookSubscriptionFilter"/> class.
		/// </summary>
		/// <param name="format">
		/// The format of the filter.
		/// </param>
		/// <param name="filters">
		/// An initial list of filter expressions.
		/// </param>
		public WebhookSubscriptionFilter(string format, IEnumerable<string> filters)
			: this(false, format, filters) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WebhookSubscriptionFilter"/> class
		/// </summary>
		/// <param name="format">
		/// The format of the filter.
		/// </param>
		public WebhookSubscriptionFilter(string format)
			: this(format, new List<string>()) {
		}

		/// <summary>
		/// Gets the format of the filter.
		/// </summary>
		public string FilterFormat { get; }

		/// <summary>
		/// Gets a read-only collection of the filter expressions.
		/// </summary>
		public IReadOnlyCollection<string> Filters => filters.AsReadOnly();

		/// <summary>
		/// Gets a value indicating if the filter is empty.
		/// </summary>
		public bool IsEmpty => filters.Count == 0;

		/// <summary>
		/// Gets a value indicating if the filter is a wildcard.
		/// </summary>
		public bool IsWildcard => filters.Count == 1 && WebhookFilter.IsWildcard(filters[0]);

		/// <summary>
		/// Represents an empty filter.
		/// </summary>
		public static WebhookSubscriptionFilter Empty => new WebhookSubscriptionFilter(true, "<empty>", null);

		/// <summary>
		/// Adds a new expression to the filter.
		/// </summary>
		/// <param name="expression">
		/// The filter expression to add.
		/// </param>
		/// <exception cref="NotSupportedException">
		/// Thrown when the filter is read-only.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown when the given <paramref name="expression"/> is <c>null</c> or empty.
		/// </exception>
		public void AddFilter(string expression) {
			if (string.IsNullOrWhiteSpace(expression)) 
				throw new ArgumentException($"'{nameof(expression)}' cannot be null or whitespace.", nameof(expression));

			if (readOnly)
				throw new NotSupportedException("The request is read-only");

			lock (filters) {
				filters.Add(expression);
			}
		}

		/// <summary>
		/// Creates a new instance of the <see cref="WebhookSubscriptionFilter"/> class
		/// </summary>
		/// <param name="format">
		/// The format of the expression.
		/// </param>
		/// <param name="filters">
		/// The list of filter expressions.
		/// </param>
		/// <returns>
		/// Returns a new instance of the <see cref="WebhookSubscriptionFilter"/> class.
		/// </returns>
		public static WebhookSubscriptionFilter Create(string format, params string[] filters) {
			return new WebhookSubscriptionFilter(format, filters);
		}
	}
}
