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
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public class WebhookSubscriptionValidator<TSubscription> : IWebhookSubscriptionValidator<TSubscription> where TSubscription : class, IWebhookSubscription {
		public virtual Task<WebhookValidationResult> ValidateAsync(WebhookSubscriptionManager<TSubscription> manager, TSubscription subscription, CancellationToken cancellationToken) {
			cancellationToken.ThrowIfCancellationRequested();

			var result = Validate(manager, subscription);

			return Task.FromResult(result);
		}

		public virtual WebhookValidationResult Validate(WebhookSubscriptionManager<TSubscription> manager, TSubscription subscription) {
			if (String.IsNullOrWhiteSpace(subscription.DestinationUrl))
				return WebhookValidationResult.Failed("The destination URL of the webhook is missing");

			if (!Uri.TryCreate(subscription.DestinationUrl, UriKind.Absolute, out var url))
				return WebhookValidationResult.Failed("The destination URL format is invalid");

			// TODO: obtain the configuration of supported delivery channels: for the moment only HTTP(S)
			// in future implementations we might extend this to support more channels
			if (url.Scheme != Uri.UriSchemeHttps &&
				url.Scheme != Uri.UriSchemeHttp)
				return WebhookValidationResult.Failed($"URL scheme '{url.Scheme}' not supported");

			return WebhookValidationResult.Success;
		}
	}
}
