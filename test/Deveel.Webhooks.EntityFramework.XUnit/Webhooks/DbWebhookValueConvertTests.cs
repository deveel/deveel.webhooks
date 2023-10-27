namespace Deveel.Webhooks {
	public static class DbWebhookValueConvertTests {
		[Theory]
		[InlineData(null, null)]
		[InlineData(true, "true")]
		[InlineData(false, "false")]
		[InlineData(123, "123")]
		[InlineData(123L, "123")]
		[InlineData(123.45, "123.45")]
		[InlineData(123.45f, "123.45")]
		[InlineData("foo", "foo")]
		public static void ConvertToString(object? value, string expected) {
			var actual = DbWebhookValueConvert.ConvertToString(value);
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(null, DbWebhookValueTypes.String)]
		[InlineData(true, DbWebhookValueTypes.Boolean)]
		[InlineData(false, DbWebhookValueTypes.Boolean)]
		[InlineData(123, DbWebhookValueTypes.Integer)]
		[InlineData(123L, DbWebhookValueTypes.Integer)]
		[InlineData(123.45, DbWebhookValueTypes.Number)]
		[InlineData(123.45f, DbWebhookValueTypes.Number)]
		[InlineData("foo", DbWebhookValueTypes.String)]
		public static void GetValueType(object? value, string expected) {
			var actual = DbWebhookValueConvert.GetValueType(value);
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(null, DbWebhookValueTypes.String, null)]
		[InlineData("true", DbWebhookValueTypes.Boolean, true)]
		[InlineData("false", DbWebhookValueTypes.Boolean, false)]
		[InlineData("123", DbWebhookValueTypes.Integer, 123)]
		[InlineData("123", DbWebhookValueTypes.Number, 123.0)]
		[InlineData("123.45", DbWebhookValueTypes.Number, 123.45)]
		[InlineData("123.45678901", DbWebhookValueTypes.Number, 123.45678901d)]
		[InlineData("foo", DbWebhookValueTypes.String, "foo")]
		public static void GetValue(string? value, string valueType, object? expected) {
			var actual = DbWebhookValueConvert.Convert(value, valueType);
			Assert.Equal(expected, actual);
		}
	}
}
