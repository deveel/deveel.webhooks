using Xunit;

namespace Deveel.Webhooks {
	public static class EventNotificationTests {
		[Fact]
		public static void NewNotification_OneEvent() {
			var eventInfo = new EventInfo("subj1", "test.event");
			var notification = new EventNotification(eventInfo);

			Assert.Equal("test.event", notification.EventType);
			Assert.Single(notification.Events);
			Assert.Equal(eventInfo, notification.Events[0]);
			Assert.Single(notification);
		}

		[Fact]
		public static void NewNotification_MultipleEvents() {
			var eventInfo1 = new EventInfo("subj1", "test.event");
			var eventInfo2 = new EventInfo("subj2", "test.event");
			var notification = new EventNotification("test.event", new[] {eventInfo1, eventInfo2});

			Assert.Equal("test.event", notification.EventType);
			Assert.Equal(2, notification.Events.Count);
			Assert.Equal(eventInfo1, notification.Events[0]);
			Assert.Equal(eventInfo2, notification.Events[1]);
		}

		[Fact]
		public static void NewNotification_InvalidEventType() {
			var eventInfo1 = new EventInfo("subj1", "test.event");
			var eventInfo2 = new EventInfo("subj2", "test.event");
			Assert.Throws<ArgumentException>(() => new EventNotification("test.event2", new[] {eventInfo1, eventInfo2}));
		}

		[Fact]
		public static void NewNotification_EmptyEvents() {
			Assert.Throws<ArgumentException>(() => new EventNotification("test.event", Array.Empty<EventInfo>()));
		}

		[Fact]
		public static void NewNotification_EmptyEventType() {
			Assert.Throws<ArgumentException>(() => new EventNotification("", new[] {new EventInfo("sub1", "test.event")}));
		}
	}
}
