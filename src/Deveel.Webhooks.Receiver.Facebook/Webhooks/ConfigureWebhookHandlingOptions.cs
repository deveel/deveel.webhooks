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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deveel.Webhooks.Facebook;
using Microsoft.Extensions.Options;

namespace Deveel.Webhooks
{
    class ConfigureWebhookHandlingOptions : IPostConfigureOptions<WebhookHandlingOptions> {
		public void PostConfigure(string name, WebhookHandlingOptions options) {
			if (String.Equals(name, nameof(FacebookWebhook))) {
				options.ResponseStatusCode = 200;
				options.InvalidStatusCode = 400;
			}
		}
	}
}
