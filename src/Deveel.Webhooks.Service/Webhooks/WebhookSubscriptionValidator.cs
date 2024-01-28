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

using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

using Deveel.Data;

namespace Deveel.Webhooks {
	/// <summary>
	/// A default implementation of the <see cref="IWebhookSubscriptionValidator{TSubscription}"/>
	/// </summary>
	/// <typeparam name="TSubscription">
	/// The type of the subscription that is validated.
	/// </typeparam>
	public class WebhookSubscriptionValidator<TSubscription> : IWebhookSubscriptionValidator<TSubscription> where TSubscription : class, IWebhookSubscription {
		/// <summary>
		/// Validates the given <paramref name="subscription"/> to have
		/// the required properties to be registered
		/// </summary>
		/// <param name="manager">
		/// The manager that is used to validate the subscription
		/// </param>
		/// <param name="subscription">
		/// The webhook subscription entity to validate
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token used to cancel the validation
		/// </param>
		/// <returns>
		/// Returns an <see cref="IAsyncEnumerable{T}"/> that yields
		/// all the validation results.
		/// </returns>
		public virtual async IAsyncEnumerable<ValidationResult> ValidateAsync(EntityManager<TSubscription> manager, TSubscription subscription, [EnumeratorCancellation] CancellationToken cancellationToken) {
			cancellationToken.ThrowIfCancellationRequested();

			if (!ValidateUrl(subscription, out var result))
				yield return result!;

			await Task.CompletedTask;
		}

		private static bool ValidateUrl(TSubscription subscription, out ValidationResult? result) {
			if (String.IsNullOrWhiteSpace(subscription.DestinationUrl)) {
				result = new ValidationResult("The destination URL of the webhook is missing", new[] { nameof(IWebhookSubscription.DestinationUrl) });
				return false;
			}

			if (!Uri.TryCreate(subscription.DestinationUrl, UriKind.Absolute, out var url)) {
				result = new ValidationResult("The destination URL format is invalid", new[] { nameof(IWebhookSubscription.DestinationUrl)});
				return false;
			}

			if (url == null) {
				result = new ValidationResult("The destination URL format is invalid", new[] { nameof(IWebhookSubscription.DestinationUrl) });
				return false;
			}

			// TODO: obtain the configuration of supported delivery channels: for the moment only HTTP(S)
			// in future implementations we might extend this to support more channels

			if (url.Scheme != Uri.UriSchemeHttps &&
				url.Scheme != Uri.UriSchemeHttp) {
				result = new ValidationResult($"URL scheme '{url.Scheme}' not supported", new[] { nameof(IWebhookSubscription.DestinationUrl) });
				return false;
			}

			result = null;
			return true;
		}
	}
}
