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
	public interface IWebhookDeliveryOptionsBuilder {
		IWebhookDeliveryOptionsBuilder IncludeFields(WebhookFields fields);

		IWebhookDeliveryOptionsBuilder MaxAttempts(int count);

		IWebhookDeliveryOptionsBuilder TimeOutAfter(TimeSpan timeSpan);

		IWebhookDeliveryOptionsBuilder BodyFormat(string format);

		IWebhookDeliveryOptionsBuilder JsonSerialization(JsonSerializerSettings settings);

		IWebhookDeliveryOptionsBuilder SignWebhooks(bool sign);

		IWebhookDeliveryOptionsBuilder SigningAlgorithm(string algorithm);

		IWebhookDeliveryOptionsBuilder SignatureHeaderName(string headerName);

		IWebhookDeliveryOptionsBuilder SignatureQueryStringKey(string key);

		IWebhookDeliveryOptionsBuilder SignatureLocation(WebhookSignatureLocation location);
	}
}
