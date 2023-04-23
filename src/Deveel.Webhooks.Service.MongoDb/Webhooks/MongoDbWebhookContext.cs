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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoFramework;

namespace Deveel.Webhooks {
	public class MongoDbWebhookContext : MongoDbContext, IMongoDbWebhookContext {
		public MongoDbWebhookContext(IMongoDbConnection connection) : base(connection) {
		}

		protected override void OnConfigureMapping(MappingBuilder mappingBuilder) {
			mappingBuilder.Entity<MongoWebhookSubscription>();
			mappingBuilder.Entity<MongoWebhookDeliveryResult>();

			base.OnConfigureMapping(mappingBuilder);
		}
	}
}
