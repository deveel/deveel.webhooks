namespace Deveel.Webhooks {
	/// <summary>
	/// Provides the types of values that can be
	/// handled by a webhook subscription property.
	/// </summary>
	public static class DbWebhookValueTypes {
		/// <summary>
		/// A string value.
		/// </summary>
		public const string String = "string";

		/// <summary>
		/// A numeric value, that can be either an integer or a floating point.
		/// </summary>
		public const string Number = "number";

		/// <summary>
		/// A boolean value.
		/// </summary>
		public const string Boolean = "bool";

		/// <summary>
		/// An integer number value.
		/// </summary>
		public const string Integer = "integer";

		/// <summary>
		/// A date-time value.
		/// </summary>
		public const string DateTime = "datetime";
	}
}
