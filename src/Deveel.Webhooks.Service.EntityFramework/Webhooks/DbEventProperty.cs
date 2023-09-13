namespace Deveel.Webhooks {
	public class DbEventProperty {
		public int? Id { get; set; }

		public string Key { get; set; }

		public string? Value { get; set; }

		public string ValueType { get; set; }

		public int? EventId { get; set; }

		public DbEventInfo? Event { get; set; }

		public object? GetValue() => DbWebhookValueUtil.GetValue(Value, ValueType);
	}
}
