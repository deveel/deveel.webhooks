// Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations.Schema;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides an implementation of <see cref="IEventInfo"/> that is
	/// capable of being stored in a MongoDB database.
	/// </summary>
	public class MongoEventInfo : IEventInfo {
		/// <inheritdoc/>
		[Column("subject")]
		public string Subject { get; set; }

		/// <inheritdoc/>
		[Column("event_type")]
		public string EventType { get; set; }

		/// <inheritdoc/>
		[Column("event_id")]
		public string EventId { get; set; }

		string IEventInfo.Id => EventId;

		/// <inheritdoc/>
		[Column("timestamp")]
		public DateTimeOffset TimeStamp { get; set; }

		/// <inheritdoc/>
		[Column("data_version")]
		public string? DataVersion { get; set; }

		object? IEventInfo.Data => EventData;

		/// <inheritdoc/>
		[Column("data")]
		public virtual BsonDocument EventData { get; set; }

		/// <inheritdoc/>
		[Column("properties")]
		[BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
		public virtual IDictionary<string, object?> Properties { get; set; } = new Dictionary<string, object?>();
	}
}
