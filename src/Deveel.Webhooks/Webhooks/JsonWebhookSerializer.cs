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
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Deveel.Webhooks {
	public sealed class JsonWebhookSerializer : IWebhookSerializer {
		private readonly WebhookDeliveryOptions options;
		private WebhookDeliveryOptions value;

		public JsonWebhookSerializer(IOptions<WebhookDeliveryOptions> options)
			: this(options.Value) {
		}

		public JsonWebhookSerializer(WebhookDeliveryOptions options) {
			this.options = options;
		}

		string IWebhookSerializer.Format => "json";

		private WebhookPayload GetWebhookPayload(IWebhook webhook) {
			var payload = new WebhookPayload();

			var fields = options?.IncludeFields ?? WebhookFields.All;

			if ((fields & WebhookFields.EventId) != 0)
				payload.EventId = webhook.Id;
			if ((fields & WebhookFields.EventName) != 0)
				payload.EventType = webhook.EventType;
			if ((fields & WebhookFields.TimeStamp) != 0)
				payload.TimeStamp = webhook.TimeStamp;
			if ((fields & WebhookFields.Name) != 0)
				payload.WebhookName = webhook.Name;

			// first normalize the string to the JSON serialization settings
			var dataJson = JsonConvert.SerializeObject(webhook.Data, options.JsonSerializerSettings);

			payload.Data = JObject.Parse(dataJson);

			return payload;
		}


		public Task<string> GetAsStringAsync(IWebhook webhook, CancellationToken cancellationToken) {
			var payload = GetWebhookPayload(webhook);

			var serializedBody = options.JsonSerializerSettings != null
				? JsonConvert.SerializeObject(payload, options.JsonSerializerSettings)
				: JsonConvert.SerializeObject(payload);

			return Task.FromResult(serializedBody);
		}

		public async Task WriteAsync(HttpRequestMessage requestMessage, IWebhook webhook, CancellationToken cancellationToken) {
			var serializedBody = await GetAsStringAsync(webhook, cancellationToken);

			requestMessage.Content = new StringContent(serializedBody, Encoding.UTF8, "application/json");
		}
	}
}
