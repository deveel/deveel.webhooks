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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using Newtonsoft.Json.Linq;

namespace Deveel.Webhooks {
	public class DefaultWebhookReceiver<T> : IWebhookReceiver<T> where T : class {
		private readonly Action<JObject, T> afterRead;

		public DefaultWebhookReceiver(IOptions<WebhookReceiveOptions> options, Action<JObject, T> afterRead)
			: this(options?.Value, afterRead) {
		}

		public DefaultWebhookReceiver(IOptions<WebhookReceiveOptions> options)
			: this(options, null) {
		}

		public DefaultWebhookReceiver(WebhookReceiveOptions options, Action<JObject, T> afterRead) {
			Options = options;
			this.afterRead = afterRead;
		}

		public DefaultWebhookReceiver(WebhookReceiveOptions options)
			: this(options, null) {
		}

		public DefaultWebhookReceiver()
			: this(new WebhookReceiveOptions()) {
		}

		protected WebhookReceiveOptions Options { get; }

		protected virtual void OnAfterRead(JObject json, T obj) {
			afterRead?.Invoke(json, obj);
		}

		public Task<T> ReceiveAsync(HttpRequest request, CancellationToken cancellationToken) {
			return request.GetWebhookAsync<T>(Options, OnAfterRead, cancellationToken);
		}
	}
}
