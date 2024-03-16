using Deveel.Data;
using Deveel.Webhooks.Mapping;
using Deveel.Webhooks.Models;

using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;

namespace Deveel.Webhooks.Controllers {
	[ApiController]
	[Route("subscriptions")]
	[Produces("application/json")]
	public class SubscriptionController : ControllerBase {
		private readonly WebhookSubscriptionManager<MongoWebhookSubscription, ObjectId> subscriptionManager;

		public SubscriptionController(WebhookSubscriptionManager<MongoWebhookSubscription, ObjectId> subscriptionManager) {
			this.subscriptionManager = subscriptionManager;
		}

		[HttpGet("{id}")]
		[ProducesResponseType(typeof(MongoWebhookSubscription), 200)]
		public async Task<IActionResult> Get([FromRoute] string id) {
			var subscription = await subscriptionManager.FindAsync(ObjectId.Parse(id));
			if (subscription == null)
				return NotFound();

			return Ok(subscription.AsModel());
		}

		[HttpPost]
		[ProducesResponseType(typeof(MongoWebhookSubscription), 201)]
		public async Task<IActionResult> Post([FromBody] WebhookSubscriptionModel model) {
			var subscription = model.AsEntity();
			await subscriptionManager.AddAsync(subscription);

			return CreatedAtAction(nameof(Get), new {id = subscription.Id}, subscription.AsModel());
		}

		[HttpPut("{id}")]
		[ProducesResponseType(typeof(MongoWebhookSubscription), 200)]
		public async Task<IActionResult> Put([FromRoute] string id, [FromBody] WebhookSubscriptionModel model) {
			var subscription = await subscriptionManager.FindAsync(ObjectId.Parse(id));
			if (subscription == null)
				return NotFound();

			subscription.Update(model);

			await subscriptionManager.UpdateAsync(subscription);

			return Ok(subscription.AsModel());
		}

		[HttpDelete("{id}")]
		[ProducesResponseType(204)]
		public async Task<IActionResult> Delete([FromRoute] string id) {
			var subscription = await subscriptionManager.FindAsync(ObjectId.Parse(id));
			if (subscription == null)
				return NotFound();

			await subscriptionManager.RemoveAsync(subscription);
			return NoContent();
		}

		[HttpGet]
		[ProducesResponseType(typeof(WebhookSubscriptionPageModel), 200)]
		public async Task<IActionResult> GetPage([FromQuery] int? page, [FromQuery] int? size) {
			var query = new PageQuery<MongoWebhookSubscription>(page ?? 1, size ?? 10);

			var pageResult = await subscriptionManager.GetPageAsync(query);
			var model = new WebhookSubscriptionPageModel {
				Page = page ?? 1,
				PageSize = size ?? 10,
				TotalItems = pageResult.TotalItems,
				TotalPages = pageResult.TotalPages,
				Items = pageResult.Items?.Select(x => x.AsModel()).ToArray()
			};

			return Ok(model);
		}
	}
}
