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

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Deveel.Webhooks {
    class WebhookRequestVerfierMiddleware<TWebhook>  where TWebhook : class {
        private readonly RequestDelegate next;
        private readonly ILogger logger;

        public WebhookRequestVerfierMiddleware(
            RequestDelegate next,
            ILogger<WebhookRequestVerfierMiddleware<TWebhook>>? logger = null) {
            this.next = next;
            this.logger = logger ?? NullLogger<WebhookRequestVerfierMiddleware<TWebhook>>.Instance;
        }

        public async Task InvokeAsync(HttpContext context) {
            try {
                var verifier = context.RequestServices.GetRequiredService<IWebhookRequestVerifier<TWebhook>>();

				logger.TraceVerificationRequest();

                var result = await verifier.VerifyRequestAsync(context.Request, context.RequestAborted);

				if (result != null) {
					if (result.IsVerified) {
						logger.TraceSuccessVerification();
					} else {
						logger.WarnVerificationFailed();
					}
				}

                await next.Invoke(context);

                if (!context.Response.HasStarted && result != null) {
					await verifier.HandleResultAsync(result, context.Response, context.RequestAborted);
                }
            } catch (WebhookReceiverException ex) {
				logger.LogUnhandledReceiveError(ex);

                if (!context.Response.HasStarted) {
                    // TODO: make this configurable...
					context.Response.StatusCode = 500;

					// TODO: Should we emit anything here?
					await context.Response.WriteAsync("");
                }
            }
        }
	}
}
