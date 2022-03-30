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

using Newtonsoft.Json;

namespace Deveel.Webhooks {
	public sealed class WebhookDeliveryOptionsBuilder : IWebhookDeliveryOptionsBuilder {
		public WebhookDeliveryOptionsBuilder(WebhookDeliveryOptions options) {
			Options = options;
		}

		public WebhookDeliveryOptions Options { get; }

		public IWebhookDeliveryOptionsBuilder IncludeFields(WebhookFields fields) {
			Options.IncludeFields = fields;
			return this;
		}

		public IWebhookDeliveryOptionsBuilder JsonSerialization(JsonSerializerSettings settings) {
			Options.JsonSerializerSettings = settings;
			return this;
		}

		public IWebhookDeliveryOptionsBuilder BodyFormat(string format) {
			if (string.IsNullOrWhiteSpace(format)) 
				throw new ArgumentException($"'{nameof(format)}' cannot be null or whitespace.", nameof(format));

			Options.BodyFormat = format;
			return this;
		}

		public IWebhookDeliveryOptionsBuilder MaxAttempts(int count) {
			if (count <= 0)
				throw new ArgumentException("Cannot set a negative number of attempts");

			Options.MaxAttemptCount = count;
			return this;
		}

		public IWebhookDeliveryOptionsBuilder SignatureHeaderName(string headerName) {
			if (string.IsNullOrWhiteSpace(headerName)) 
				throw new ArgumentException($"'{nameof(headerName)}' cannot be null or whitespace.", nameof(headerName));

			Options.SignatureLocation = WebhookSignatureLocation.Header;
			Options.SignatureHeaderName = headerName;
			return this;
		}

		public IWebhookDeliveryOptionsBuilder SignatureLocation(WebhookSignatureLocation location) {
			Options.SignatureLocation = location;
			return this;
		}

		public IWebhookDeliveryOptionsBuilder SignatureQueryStringKey(string key) {
			if (string.IsNullOrWhiteSpace(key))
				throw new ArgumentException($"'{nameof(key)}' cannot be null or whitespace.", nameof(key));

			Options.SignatureQueryStringKey = key;
			Options.SignatureLocation = WebhookSignatureLocation.QueryString;
			return this;
		}

		public IWebhookDeliveryOptionsBuilder SigningAlgorithm(string algorithm) {
			if (string.IsNullOrWhiteSpace(algorithm))
				throw new ArgumentException($"'{nameof(algorithm)}' cannot be null or whitespace.", nameof(algorithm));

			Options.SignatureAlgorithm = algorithm;
			return this;
		}

		public IWebhookDeliveryOptionsBuilder SignWebhooks(bool sign) {
			Options.SignWebhooks = sign;
			return this;
		}

		public IWebhookDeliveryOptionsBuilder TimeOutAfter(TimeSpan timeSpan) {
			if (timeSpan == TimeSpan.Zero)
				throw new ArgumentException("The time-out cannot be zero", nameof(timeSpan));

			Options.TimeOut = timeSpan;
			return this;
		}
	}
}
