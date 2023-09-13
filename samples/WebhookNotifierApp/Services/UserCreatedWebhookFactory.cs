using Deveel.Webhooks.Models;

namespace Deveel.Webhooks.Services {
	public class UserCreatedWebhookFactory : IWebhookFactory<IdentityWebhook> {
		private readonly IUserResolver userResolver;

		public UserCreatedWebhookFactory(IUserResolver userResolver) {
			this.userResolver = userResolver;
		}

		public async Task<IdentityWebhook> CreateAsync(IWebhookSubscription subscription, EventInfo eventInfo, CancellationToken cancellationToken = default) {
			var userCreated = (UserCreatedEvent?)eventInfo.Data;
			var user = await userResolver.ResolveUserAsync(userCreated!.UserId, cancellationToken);

			if (user == null)
				throw new InvalidOperationException();

			return new IdentityWebhook {
				EventId = eventInfo.Id,
				EventType = "user_created",
				TimeStamp = eventInfo.TimeStamp,
				User = user
			};
		}
	}
}
