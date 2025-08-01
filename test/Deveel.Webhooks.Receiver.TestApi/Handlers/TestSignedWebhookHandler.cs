﻿// Copyright 2022-2025 Antonello Provenzano
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

using Newtonsoft.Json;

namespace Deveel.Webhooks.Handlers {
	public class TestSignedWebhookHandler : IWebhookHandler<TestSignedWebhook> {
		private readonly IWebhookCallback<TestSignedWebhook> _callback;

		public TestSignedWebhookHandler(IWebhookCallback<TestSignedWebhook> callback) {
			_callback = callback;
		}

		public Task HandleAsync(TestSignedWebhook webhook, CancellationToken cancellationToken = default) {
			_callback.OnWebhookHandled(webhook);

			return Task.CompletedTask;
		}
	}
}
