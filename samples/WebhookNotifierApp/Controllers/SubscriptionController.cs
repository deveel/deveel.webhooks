using Deveel.Webhooks.Mapping;
using Deveel.Webhooks.Models;

using Microsoft.AspNetCore.Mvc;

namespace Deveel.Webhooks.Controllers {
	[ApiController]
	[Route("subscriptions")]
	[Produces("application/json")]
	public class SubscriptionController : ControllerBase {
		private readonly WebhookSubscriptionManager<MongoWebhookSubscription> subscriptionManager;

		public SubscriptionController(WebhookSubscriptionManager<MongoWebhookSubscription> subscriptionManager) {
			this.subscriptionManager = subscriptionManager;
		}

		[HttpGet("{id}")]
		[ProducesResponseType(typeof(MongoWebhookSubscription), 200)]
		public async Task<IActionResult> GetAsync([FromRoute] string id) {
			var subscription = await subscriptionManager.FindByIdAsync(id);
			if (subscription == null)
				return NotFound();

			return Ok(subscription.AsModel());
		}

		[HttpPost]
		[ProducesResponseType(typeof(MongoWebhookSubscription), 201)]
		public async Task<IActionResult> PostAsync([FromBody] WebhookSubscriptionModel model) {
			var subscription = model.AsEntity();
			await subscriptionManager.CreateAsync(subscription);

			return CreatedAtAction(nameof(GetAsync), new {id = subscription.Id}, subscription.AsModel());
		}

		[HttpPut("{id}")]
		[ProducesResponseType(typeof(MongoWebhookSubscription), 200)]
		public async Task<IActionResult> PutAsync([FromRoute] string id, [FromBody] WebhookSubscriptionModel model) {
			var subscription = await subscriptionManager.FindByIdAsync(id);
			if (subscription == null)
				return NotFound();

			subscription.Update(model);

			await subscriptionManager.UpdateAsync(subscription);

			return Ok(subscription.AsModel());
		}

		[HttpDelete("{id}")]
		[ProducesResponseType(204)]
		public async Task<IActionResult> DeleteAsync([FromRoute] string id) {
			var subscription = await subscriptionManager.FindByIdAsync(id);
			if (subscription == null)
				return NotFound();

			await subscriptionManager.DeleteAsync(subscription);
			return NoContent();
		}

		[HttpGet]
		[ProducesResponseType(typeof(WebhookSubscriptionPageModel), 200)]
		public async Task<IActionResult> GetPageAsync([FromQuery] int? page, [FromQuery] int? size) {
			var query = new PagedQuery<MongoWebhookSubscription>(page ?? 1, size ?? 10);

			var pageResult = await subscriptionManager.GetPageAsync(query);
			var model = new WebhookSubscriptionPageModel {
				Page = page ?? 1,
				PageSize = size ?? 10,
				TotalItems = pageResult.TotalCount,
				TotalPages = pageResult.TotalPages,
				Items = pageResult.Items.Select(x => x.AsModel()).ToArray()
			};

			return Ok(model);
		}
	}
}
