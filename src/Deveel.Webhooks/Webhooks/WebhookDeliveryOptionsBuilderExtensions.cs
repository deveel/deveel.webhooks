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
	public static class WebhookDeliveryOptionsBuilderExtensions {
		public static IWebhookDeliveryOptionsBuilder SignWithHmacSha256(this IWebhookDeliveryOptionsBuilder builder)
			=> builder.SignWebhooks(true)
					  .SigningAlgorithm(WebhookSignatureAlgorithms.HmacSha256);

		public static IWebhookDeliveryOptionsBuilder SignWebhooks(this IWebhookDeliveryOptionsBuilder builder)
			=> builder.SignWebhooks(true);

		public static IWebhookDeliveryOptionsBuilder DoNotSignWebhooks(this IWebhookDeliveryOptionsBuilder builder)
			=> builder.SignWebhooks(false);

		public static IWebhookDeliveryOptionsBuilder PlaceSignatureInHeaders(this IWebhookDeliveryOptionsBuilder builder, string headerName = "X-WEBHOOK-SIGNATURE")
			=> builder.SignatureLocation(WebhookSignatureLocation.Header)
					  .SignatureHeaderName(headerName);

		public static IWebhookDeliveryOptionsBuilder PlaceSignatureInQueryStrings(this IWebhookDeliveryOptionsBuilder builder, string key = "webhook-signature")
			=> builder.SignatureLocation(WebhookSignatureLocation.QueryString)
					  .SignatureQueryStringKey(key);

		public static IWebhookDeliveryOptionsBuilder SecondsBeforeTimeOut(this IWebhookDeliveryOptionsBuilder builder, int seconds)
			=> builder.TimeOutAfter(TimeSpan.FromSeconds(seconds));

		public static IWebhookDeliveryOptionsBuilder MillisecondsBeforeTimeOut(this IWebhookDeliveryOptionsBuilder builder, int millis)
			=> builder.TimeOutAfter(TimeSpan.FromMilliseconds(millis));

		public static IWebhookDeliveryOptionsBuilder IncludeAllFields(this IWebhookDeliveryOptionsBuilder builder)
			=> builder.IncludeFields(WebhookFields.All);

		public static IWebhookDeliveryOptionsBuilder DoNotIncludeAnyField(this IWebhookDeliveryOptionsBuilder builder)
			=> builder.IncludeFields(WebhookFields.None);
	}
}
