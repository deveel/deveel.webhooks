using System.Globalization;

namespace Deveel.Webhooks {
	public static class DbWebhookValueConvert {
		public static string GetValueType(object? value) {
			if (value is null || value is string)
				return DbWebhookValueTypes.String;
			if (value is int || value is long)
				return DbWebhookValueTypes.Integer;
			if (value is bool)
				return DbWebhookValueTypes.Boolean;
			if (value is double || value is float)
				return DbWebhookValueTypes.Number;
			if (value is DateTime)
				return DbWebhookValueTypes.DateTime;

			throw new NotSupportedException($"The value of type '{value.GetType()}' is not supported");
		}

		public static string? ConvertToString(object? value) {
			if (value is null)
				return null;
			if (value is bool b)
				return b ? "true" : "false";
			if (value is string str)
				return str;
			if (value is int || value is long)
				return System.Convert.ToString(value, CultureInfo.InvariantCulture);
			if (value is double || value is float)
				return System.Convert.ToString(value, CultureInfo.InvariantCulture);
			if (value is DateTime dt)
				return dt.ToString("O");

			throw new NotSupportedException($"The value of type '{value.GetType()}' is not supported");
		}

		public static object? Convert(string? value, string valueType) {
			if (value is null)
				return null;

			return valueType switch {
				DbWebhookValueTypes.Boolean => ParseBoolean(value),
				DbWebhookValueTypes.Integer => Int64.Parse(value, CultureInfo.InvariantCulture),
				DbWebhookValueTypes.Number => Double.Parse(value, CultureInfo.InvariantCulture),
				DbWebhookValueTypes.String => value,
				DbWebhookValueTypes.DateTime => DateTime.Parse(value, CultureInfo.InvariantCulture),
				_ => throw new NotSupportedException($"The value type '{valueType}' is not supported")
			};
		}

		private static bool ParseBoolean(string value) {
			return value switch {
				"true" => true,
				"false" => false,
				_ => Boolean.Parse(value)
			};
		}
	}
}
