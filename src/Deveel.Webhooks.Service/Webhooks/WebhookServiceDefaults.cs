namespace Deveel.Webhooks
{
	/// <summary>
	/// A set of default values for the webhook service.
	/// </summary>
	public static class WebhookServiceDefaults
	{
		/// <summary>
		/// Gets or sets the service domain identifier used 
		/// for webhook operations (default is "WEBHOOKS").
		/// </summary>
		public static string ServiceDomain { get; set; } = "WEBHOOKS";
	}
}
