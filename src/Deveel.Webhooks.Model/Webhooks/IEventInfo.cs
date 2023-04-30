namespace Deveel.Webhooks {
	/// <summary>
	/// Defines a contract for an event that might trigger
	/// the notification of a webhook
	/// </summary>
	public interface IEventInfo {
		/// <summary>
		/// Gets the subject of the event (e.g. the name of the aggregate)
		/// </summary>
		string Subject { get; }

		/// <summary>
		/// Gets the type of event that occurred.
		/// </summary>
		string EventType { get; }

		/// <summary>
		/// Gets a unique identifier of the event from the remote system
		/// </summary>
		string Id { get; }

		/// <summary>
		/// Gets the exact time the event occurred
		/// </summary>
		DateTimeOffset TimeStamp { get; }

		/// <summary>
		/// Gets the version of the data carried by the event
		/// </summary>
		string? DataVersion { get; }

		/// <summary>
		/// Gets a set of data carried by the event
		/// </summary>
		object? Data { get; }
	}
}
