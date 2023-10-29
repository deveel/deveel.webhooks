namespace Deveel.Webhooks.Models {
	public class WebhookSubscriptionPageModel {
		public int Page { get; set; }

		public int PageSize { get; set; }

		public int TotalPages { get; set; }

		public int TotalItems { get; set; }

		public WebhookSubscriptionModel[]? Items { get; set; }
	}
}
