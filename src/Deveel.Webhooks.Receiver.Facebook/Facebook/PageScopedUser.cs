using System;
using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	public sealed class PageScopedUser {
		[JsonConstructor]
		public PageScopedUser(string id, string? commentId = null, string? postId = null) {
			if (string.IsNullOrWhiteSpace(id))
				throw new ArgumentException($"'{nameof(id)}' cannot be null or whitespace.", nameof(id));

			Id = id;
			CommentId = commentId;
			PostId = postId;
		}

		[JsonPropertyName("id")]
		public string Id { get; }

		[JsonPropertyName("user_ref")]
		public string? UserRef { get; set; }

		[JsonPropertyName("post_id")]
		public string? PostId { get; set; }

		[JsonPropertyName("comment_id")]
		public string? CommentId { get; set; }

		public static implicit operator PageScopedUser(string id)
			=> new PageScopedUser(id);
	}
}
