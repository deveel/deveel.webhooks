using Deveel.Webhooks.Models;

namespace Deveel.Webhooks.Handlers {
	public class UserRegisteredHandler : IWebhookHandler<IdentityWebhook> {
		private readonly ILogger logger;

		public UserRegisteredHandler(ILogger<UserRegisteredHandler> logger) {
			this.logger = logger;
		}

		public Task HandleAsync(IdentityWebhook webhook, CancellationToken cancellationToken = default) {
			if (webhook.EventName == "user.registered") {
				logger.LogInformation("The user '{UserName}' was registered", webhook.User?.Name);
			}

			return Task.CompletedTask;
		}
	}
}
