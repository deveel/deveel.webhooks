using Deveel.Webhooks.Json;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Xml.Serialization;
using System.Xml;

namespace Deveel.Webhooks {
	public static class XmlSerializationTests {
		[Fact]
		public static async Task SerializeXmlNode() {
			var serializer = new SystemWebhookXmlSerializer<TestWebhook>();

			var id = Guid.NewGuid().ToString("N");

			var webhook = new TestWebhook {
				Name = "Test Event",
				Type = "test",
				IgnoredTime = DateTime.Now,
				TimeStamp = DateTime.UtcNow,
				States = new[] {"pending", "active"},
				Id = id
			};

			var xml = await serializer.SerializeToStringAsync(webhook);

			var expected = $@"<?xml version=""1.0"" encoding=""utf-8""?><webhook name=""Test Event"" eventType=""test"" eventId=""{id}"" timestamp=""{webhook.TimeStamp.ToString("O")}""><states><state>pending</state><state>active</state></states></webhook>";
			Assert.Equal(expected, xml);
		}

		[XmlRoot("webhook")]
		public class TestWebhook {
			[XmlAttribute("name")]
			public string Name { get; set; }

			[XmlAttribute("eventType")]
			public string Type { get; set; }

			[XmlIgnore]
			public DateTime IgnoredTime { get; set; }

			[XmlArray("states")]
			[XmlArrayItem("state")]
			public string[] States { get; set; }

			[XmlAttribute("eventId")]
			public string Id { get; set; }

			[XmlAttribute("timestamp", DataType = "dateTime")]
			public DateTime TimeStamp { get; set; }

			[XmlIgnore]
			public int Count { get; set; }

			[XmlAttribute("count")]
			public string? CountString {
				get => Count.ToString();
			}
		}

	}
}
