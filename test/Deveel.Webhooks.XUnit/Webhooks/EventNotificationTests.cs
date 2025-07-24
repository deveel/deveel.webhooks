// Copyright 2022-2025 Antonello Provenzano
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
