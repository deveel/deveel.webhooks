using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Xunit;

namespace Deveel.Webhooks {
	public static class XmlSerializationTests {
		private static async Task<TestWebhook?> ParseAsync(string xml) {
			var parser = new SystemWebhookXmlParser<TestWebhook>();
			return await parser.ParseWebhookAsync(xml);
		}

		[Fact]
		public static async Task ParseSingleWebhook() {
			var result = await ParseAsync("<webhook><id>123</id><name>test</name><event>test</event></webhook>");
			Assert.NotNull(result);
			Assert.Equal("123", result.Id);
			Assert.Equal("test", result.Name);
			Assert.Equal("test", result.EventType);
		}

		[Fact]
		public static async Task ParseInvalidData() {
			await Assert.ThrowsAsync<WebhookParseException>(() => ParseAsync("<webhook><id>123</event-id><name>test</name></webhook>"));
		}

		[Fact]
		public static async Task ParseWithMissingValue() {
			var result = await ParseAsync("<webhook><id>123</id><name>test</name></webhook>");

			Assert.NotNull(result);
			Assert.Equal("123", result.Id);
			Assert.Equal("test", result.Name);
			Assert.Null(result.EventType);
		}

		[XmlRoot("webhook")]
		public class TestWebhook {
			[XmlElement("id")]
			public string Id { get; set; }

			[XmlElement("name")]
			public string Name { get; set; }

			[XmlElement("event")]
			public string EventType { get; set; }
		}
	}
}
