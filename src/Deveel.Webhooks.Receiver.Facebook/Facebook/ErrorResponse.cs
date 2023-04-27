using System;
using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	/// <summary>
	/// A response object that indicates an error has 
	/// occurred while trying to send a message
	/// </summary>
	public sealed class ErrorResponse {
		/// <summary>
		/// Constructs the response with the given error
		/// </summary>
		/// <param name="error">The instance of the error.</param>
		[JsonConstructor]
		public ErrorResponse(Error error) {
			Error = error;
		}

		/// <summary>
		/// Gets the instance of the error that has occurred.
		/// </summary>
		[JsonPropertyName("error")]
		public Error Error { get; }
	}
}
