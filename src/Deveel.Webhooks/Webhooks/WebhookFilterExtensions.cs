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
	/// Extens the <see cref="IWebhookFilter"/> to add some utility methods.
	/// </summary>
	public static class WebhookFilterExtensions {
		/// <summary>
		/// Checks if the given <paramref name="filter"/> is a wildcard filter.
		/// </summary>
		/// <param name="filter">
		/// The filter to check.
		/// </param>
		/// <returns>
		/// Returns <c>true</c> if the given <paramref name="filter"/> is a wildcard filter.
		/// </returns>
		public static bool IsWildcard(this IWebhookFilter filter) => WebhookFilter.IsWildcard(filter.Expression);
	}
}
