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
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	class WebhookRequestVerfierMiddleware<TWebhook> : IMiddleware where TWebhook : class {
        private readonly WebhookReceiverOptions options;
		private readonly IWebhookRequestVerifier<TWebhook> requestVerifier;
        private readonly ILogger logger;

        public WebhookRequestVerfierMiddleware(
            IOptionsSnapshot<WebhookReceiverOptions> options,
            IWebhookRequestVerifier<TWebhook> requestVerifier, 
            ILogger<WebhookRequestVerfierMiddleware<TWebhook>>? logger = null) {
            this.options = options.GetReceiverOptions<TWebhook>();
            this.requestVerifier = requestVerifier;
            this.logger = logger ?? NullLogger<WebhookRequestVerfierMiddleware<TWebhook>>.Instance;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next) {
            try {
                var result = await requestVerifier.VerifyRequestAsync(context.Request, context.RequestAborted);

                await next.Invoke(context);

                if (!context.Response.HasStarted) {
					await requestVerifier.HandleResultAsync(result, context.Response, context.RequestAborted);
                }
            } catch (WebhookReceiverException ex) {
                if (!context.Response.HasStarted) {
                    
                }
            }
        }
	}
}
