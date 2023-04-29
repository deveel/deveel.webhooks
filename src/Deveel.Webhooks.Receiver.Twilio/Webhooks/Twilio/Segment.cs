using Microsoft.Extensions.Primitives;

namespace Deveel.Webhooks.Twilio {
	/// <summary>
	/// Represents a segment of a message received from Twilio.
	/// </summary>
	public sealed class Segment {
		/// <summary>
		/// Gets the zero-based offset of the segment in the message.
		/// </summary>
		public int Index { get; internal set; }

		/// <summary>
		/// Gets the body text of the segment.
		/// </summary>
		public string Text { get; internal set; }

		/// <summary>
		/// Gets the unique identifier of the message part for 
		/// the segment.
		/// </summary>
		public StringValues MessageId { get; internal set; }
	}
}
