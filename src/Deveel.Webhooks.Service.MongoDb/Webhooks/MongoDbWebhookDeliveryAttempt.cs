﻿// Copyright 2022-2023 Deveel
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

namespace Deveel.Webhooks {
	public class MongoDbWebhookDeliveryAttempt : IWebhookDeliveryAttempt {
		public int? ResponseStatusCode { get; set; }

		public string ResponseMessage { get; set; }

		public DateTimeOffset StartedAt { get; set; }

		public DateTimeOffset? EndedAt { get; set; }
	}
}
