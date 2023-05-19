using Deveel.Webhooks.Models;

using Microsoft.AspNetCore.Mvc;

namespace Deveel.Webhooks.Controllers {
	[Route("events/identity")]
	[ApiController]
	public class IdentityEventController : ControllerBase {
		private readonly IWebhookNotifier<IdentityWebhook> notifier;

		public IdentityEventController(IWebhookNotifier<IdentityWebhook> notifier) {
			this.notifier = notifier;
		}

		[HttpPost("{tenantId}")]
		public async Task<IActionResult> PostAsync([FromRoute] string tenantId, [FromBody] UserCreatedEvent userCreated) {
			var eventInfo = userCreated.AsEventInfo();
			var result = await notifier.NotifyAsync(eventInfo, HttpContext.RequestAborted);

			// TODO: output the result of the notification

			return Ok();
		}
	}
}
