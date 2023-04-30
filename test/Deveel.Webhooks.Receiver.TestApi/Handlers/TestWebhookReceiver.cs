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

using Deveel.Webhooks.Model;

namespace Deveel.Webhooks.Handlers {
	public class TestWebhookReceiver : IWebhookReceiver<TestWebhook> {
		public async Task<WebhookReceiveResult<TestWebhook>> ReceiveAsync(HttpRequest request, CancellationToken cancellationToken) {
			// TODO: test the signature as well ...

			var result = await request.ReadFromJsonAsync<TestWebhook>();
			return new WebhookReceiveResult<TestWebhook>(result);
		}
	}
}
