using MongoDB.Bson;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides an implementation of <see cref="IEventInfo"/> that is
	/// capable of being stored in a MongoDB database.
	/// </summary>
	public class MongoEventInfo : IEventInfo {
		/// <inheritdoc/>
		public string Subject { get; set; }

		/// <inheritdoc/>
		public string EventType { get; set; }

		/// <inheritdoc/>
		public string EventId { get; set; }

		string IEventInfo.Id => EventId;

		/// <inheritdoc/>
		public DateTimeOffset TimeStamp { get; set; }

		/// <inheritdoc/>
		public string? DataVersion { get; set; }

		object? IEventInfo.Data => EventData;

		/// <inheritdoc/>
		public virtual BsonDocument EventData { get; set; }
	}
}
