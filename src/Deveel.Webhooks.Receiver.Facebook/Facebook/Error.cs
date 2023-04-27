using System;
using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	/// <summary>
	/// Describes an error returned by the
	/// Facebook Messenger Platform
	/// </summary>
	public sealed class Error {
		/// <summary>
		/// Constructs a new error with the given code and message
		/// </summary>
		/// <param name="code">The error code</param>
		/// <param name="message">The message describing the error</param>
		[JsonConstructor]
		public Error(string code, string message) {
			Code = code;
			Message = message;
		}

		/// <summary>
		/// Gets a human-readable description of the error.
		/// </summary>
		[JsonPropertyName("message")]
		public string Message { get; }

		/// <summary>
		/// Gets the code of the error
		/// </summary>
		[JsonPropertyName("code")]
		public string Code { get; }

		/// <summary>
		/// Gets the type of error
		/// </summary>
		[JsonPropertyName("type")]
		public string? Type { get; set; }

		/// <summary>
		/// Gets or sets an optional sub-code identifying the error
		/// </summary>
		[JsonPropertyName("error_subcode")]
		public string? ErrorSubCode { get; set; }

		/// <summary>
		/// Gets or sets a message to be displayed to the user for
		/// descriving the error
		/// </summary>
		[JsonPropertyName("error_user_msg")]
		public string? UserMessage { get; set; }

		/// <summary>
		/// Gets or sets the title of the dialog to be displayed
		/// to the user.
		/// </summary>
		[JsonPropertyName("error_user_title")]
		public string? UserTitle { get; set; }

		/// <summary>
		/// Gets or sets the session identifier used to
		/// trace the context of the error
		/// </summary>
		[JsonPropertyName("fbtrace_id")]
		public string? TraceId { get; set; }
	}
}
