using Deveel.Webhooks.Models;

namespace Deveel.Webhooks.Services {
	public class UserCreatedWebhookFactory : IWebhookFactory<IdentityWebhook> {
		private readonly IUserResolver userResolver;

		public UserCreatedWebhookFactory(IUserResolver userResolver) {
			this.userResolver = userResolver;
		}

		public async Task<IdentityWebhook> CreateAsync(IWebhookSubscription subscription, EventNotification notification, CancellationToken cancellationToken = default) {
			var @event = notification.SingleEvent;

			var userCreated = (UserCreatedEvent?)@event.Data;
			var user = await userResolver.ResolveUserAsync(userCreated!.UserId, cancellationToken);

			if (user == null)
				throw new InvalidOperationException();

			return new IdentityWebhook {
				EventId = @event.Id,
				EventType = "user_created",
				TimeStamp = @event.TimeStamp,
				User = user
			};
		}
	}
}
