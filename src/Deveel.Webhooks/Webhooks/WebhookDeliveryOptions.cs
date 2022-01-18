﻿// Copyright 2022 Deveel
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

using Newtonsoft.Json;

namespace Deveel.Webhooks {
	public class WebhookDeliveryOptions {
		public string SignatureHeaderName { get; set; } = "X-WEBHOOK-SIGNATURE";

		public string SignatureQueryStringKey { get; set; } = "webhook-signature";

		public bool SignWebhooks { get; set; } = true;

		public WebhookFields IncludeFields { get; set; } = WebhookFields.All;

		public WebhookSignatureLocation SignatureLocation { get; set; } = WebhookSignatureLocation.QueryString;

		public string SignatureAlgorithm { get; set; } = WebhookSignatureAlgorithms.HmacSha256;

		public int MaxAttemptCount { get; set; } = 3;

		public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(2);

		public JsonSerializerSettings JsonSerializerSettings { get; set; } = new JsonSerializerSettings();
	}
}
