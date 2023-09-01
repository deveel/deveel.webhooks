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

namespace Deveel.Webhooks {
    public class DefaultWebhookEntityConverter<TWebhook> : IWebhookEntityConverter<TWebhook> where TWebhook : class {
        public WebhookEntity ConvertWebhook(EventInfo eventInfo, TWebhook webhook) {
            if (webhook is IWebhook obj) {
                return new WebhookEntity {
                    WebhookId = obj.Id,
                    EventType = obj.EventType,
                    Data = WebhookJsonUtil.AsJson(obj.Data),
                    TimeStamp = obj.TimeStamp
                };
            }

            return new WebhookEntity {
                EventType = eventInfo.EventType,
                TimeStamp = eventInfo.TimeStamp,
                WebhookId = eventInfo.Id,
                Data = WebhookJsonUtil.AsJson(webhook)
            };
        }
    }
}
