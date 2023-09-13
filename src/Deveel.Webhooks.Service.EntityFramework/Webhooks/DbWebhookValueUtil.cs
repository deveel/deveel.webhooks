namespace Deveel.Webhooks {
	internal static class DbWebhookValueUtil {
		public static object? GetValue(string? value, string valueType) {
			if (value == null)
				return null;

			return valueType switch {
				"string" => value,
				"int" => int.Parse(value),
				"bool" => bool.Parse(value),
				"number" => double.Parse(value),
				_ => value
			};
		}
	}
}
