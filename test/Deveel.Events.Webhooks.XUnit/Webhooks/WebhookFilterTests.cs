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
