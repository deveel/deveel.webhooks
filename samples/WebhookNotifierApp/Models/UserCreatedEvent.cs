namespace Deveel.Webhooks.Models {
	public class UserCreatedEvent {
		public string TenantId { get; set; }

		public string UserId { get; set; }
	}
}
