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
	public static class WebhookConfigurationDefaults {
		public const string SerializationFormat = "json";

		public const string SignatureHeaderName = "X-WEBHOOK-SIGNATURE";

		public const string SignatureQueryStringKey = "webhook-signature";

		public const bool SignWebhooks = true;

		public const WebhookFields IncludedFields = WebhookFields.All;

		public const WebhookSignatureLocation SignatureLocation = WebhookSignatureLocation.QueryString;

		public const string SignatureAlgorithm = WebhookSignatureAlgorithms.HmacSha256;

		public const int MaxDeliveryAttempts = 3;
	}
}
