// Copyright 2022-2024 Antonello Provenzano
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

using Xunit;

namespace Deveel.Webhooks {
	public static class WebhookFilterTests {
		[Theory]
		[InlineData("a == 2", "linq", false)]
		[InlineData("*", "linq", true)]
		[InlineData("*a", "liquid", false)]
		public static void CheckWildcard(string expression, string format, bool expected) {
			Assert.Equal(expected, WebhookFilter.IsWildcard(expression));

			var filter = new WebhookFilter(expression, format);
			Assert.Equal(expected, filter.IsWildcard());
			Assert.Equal(expected, WebhookFilter.WildcardFilter.Equals(filter));
		}

		[Fact]
		public static void InstantiateNullExpression() {
			Assert.Throws<ArgumentException>(() => new WebhookFilter(null, "linq"));
			Assert.Throws<ArgumentException>(() => new WebhookFilter("*", null));
		}
	}
}
