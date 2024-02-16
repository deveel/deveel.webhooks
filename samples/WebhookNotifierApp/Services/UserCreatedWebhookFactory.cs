using Deveel.Webhooks.Models;

namespace Deveel.Webhooks.Services {
	public class UserCreatedWebhookFactory : IWebhookFactory<IdentityWebhook> {
		private readonly IUserResolver userResolver;

		public UserCreatedWebhookFactory(IUserResolver userResolver) {
			this.userResolver = userResolver;
		}

		public async Task<IList<IdentityWebhook>> CreateAsync(IWebhookSubscription subscription, EventNotification notification, CancellationToken cancellationToken = default) {
			var @event = notification.Events[0];

			var userCreated = (UserCreatedEvent?)@event.Data;
			var user = await userResolver.ResolveUserAsync(userCreated!.UserId, cancellationToken);

			if (user == null)
				throw new InvalidOperationException();

			return new [] { 
				new IdentityWebhook {
					EventId = @event.Id,
					EventType = @event.EventType,
					TimeStamp = @event.TimeStamp,
					User = user
				} 
			};
		}
	}
}
