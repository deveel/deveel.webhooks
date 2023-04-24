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
		/// Returns a <see cref="WebhookValidationResult"/> that indicates
		/// whether the subscription is valid or not.
		/// </returns>
		public virtual async Task<WebhookValidationResult> ValidateAsync(WebhookSubscriptionManager<TSubscription> manager, TSubscription subscription, CancellationToken cancellationToken) {
			cancellationToken.ThrowIfCancellationRequested();

            if (String.IsNullOrWhiteSpace(subscription.DestinationUrl))
                return WebhookValidationResult.Failed("The destination URL of the webhook is missing");

            if (!Uri.TryCreate(subscription.DestinationUrl, UriKind.Absolute, out var url))
                return WebhookValidationResult.Failed("The destination URL format is invalid");

            // TODO: obtain the configuration of supported delivery channels: for the moment only HTTP(S)
            // in future implementations we might extend this to support more channels
            if (url.Scheme != Uri.UriSchemeHttps &&
                url.Scheme != Uri.UriSchemeHttp)
                return WebhookValidationResult.Failed($"URL scheme '{url.Scheme}' not supported");

			await Task.CompletedTask;

            return WebhookValidationResult.Success;
		}
	}
}
