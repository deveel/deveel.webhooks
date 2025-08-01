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

using Deveel.Webhooks.Twilio;

using Microsoft.AspNetCore.Http;

namespace Deveel.Webhooks {
    class TwilioWebhookFormParser : IWebhookFormParser<TwilioWebhook> {
        public Task<TwilioWebhook> ParseWebhookAsync(IFormCollection form, CancellationToken cancellationToken) {
			try {
                var result = TwilioWebhookParser.Parse(form);
                return Task.FromResult(result);
			} catch (WebhookParseException) {
				throw;
			} catch(Exception ex) {
                throw new WebhookParseException("Unable to parse the form request into a Twilio webhook", ex);
            }
        }
    }
}
