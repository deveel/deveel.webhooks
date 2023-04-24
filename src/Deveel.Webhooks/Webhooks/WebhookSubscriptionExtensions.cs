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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	/// <summary>
	/// Extends the <see cref="IWebhookSubscription"/> to provide some helper methods.
	/// </summary>
	public static class WebhookSubscriptionExtensions {
		/// <summary>
		/// Converts a subscription to a webhook destination.
		/// </summary>
		/// <param name="subscription">
		/// The subscription to convert.
		/// </param>
		/// <returns>
		/// Returns an instance of <see cref="WebhookDestination"/> created from the information
		/// configured in the subscription.
		/// </returns>
		public static WebhookDestination AsDestination(this IWebhookSubscription subscription) {
			var destination = new WebhookDestination(subscription.DestinationUrl) {
				Headers = subscription.Headers?.ToDictionary(x => x.Key, x => x.Value)
			};

			if (!string.IsNullOrWhiteSpace(subscription.Secret)) {
				destination.Signature = new WebhookDestinationSignatureOptions {
					Secret = subscription.Secret
				};
			}

			if (!String.IsNullOrWhiteSpace(subscription.Format) &&
				Enum.TryParse<WebhookFormat>(subscription.Format, true, out var format))
				destination.Format = format;

			// TODO: Add support for other options

			return destination;
		}

		/// <summary>
		/// Converts a subscription to a webhook filter.
		/// </summary>
		/// <param name="subscription">
		/// The subscription to convert.
		/// </param>
		/// <returns>
		/// Returns an instance of <see cref="WebhookSubscriptionFilter"/> created from the filters
		/// configured in the subscription.
		/// </returns>
		/// <exception cref="InvalidOperationException"></exception>
		public static WebhookSubscriptionFilter? AsFilter(this IWebhookSubscription subscription) {
			if (subscription.Filters == null ||
				!subscription.Filters.Any())
				return null;

			var formats = subscription.Filters.Select(x => x.Format).Distinct().ToList();
			if (formats.Count > 1)
				throw new InvalidOperationException("The subscription has filters with multiple formats");

			var request = new WebhookSubscriptionFilter(formats[0]);

			foreach (var filter in subscription.Filters) {
				request.AddFilter(filter.Expression);
			}

			return request;
		}
	}
}
