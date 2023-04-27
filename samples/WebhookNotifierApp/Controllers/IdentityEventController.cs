using Deveel.Webhooks;
using Deveel.Webhooks.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Deveel.Webhooks.Controllers {
	[Route("events/identity")]
	[ApiController]
	public class IdentityEventController : ControllerBase {
		private readonly IWebhookNotifier<IdentityWebhook> notifier;

		public IdentityEventController(IWebhookNotifier<IdentityWebhook> notifier) {
			this.notifier = notifier;
		}

		[HttpPost]
		public async Task<IActionResult> PostAsync(UserCreatedEvent userCreated) {
			var eventInfo = new EventInfo("user", "created", userCreated);
			var result = await notifier.NotifyAsync(userCreated.TenantId, eventInfo, HttpContext.RequestAborted);

			// TODO: output the result of the notification

			return Ok();
		}
	}
}
