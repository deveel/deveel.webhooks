using Bogus;

namespace Deveel.Webhooks {
	public class DbWebhookDeliveryResultFaker : Faker<DbWebhookDeliveryResult> {
		public DbWebhookDeliveryResultFaker(int? eventId = null) {
			RuleFor(x => x.EventId, eventId);
			RuleFor(x => x.OperationId, f => f.Random.Guid().ToString());
			RuleFor(x => x.Webhook, f => new DbWebhookFaker().Generate());
			RuleFor(x => x.Receiver, f => new DbWebhookReceiverFaker().Generate());
			RuleFor(x => x.DeliveryAttempts, f => {
				var attemptCount = f.Random.Int(1, 5);
				var f2 = new Faker<DbWebhookDeliveryAttempt>()
					.RuleFor(x => x.ResponseStatusCode, f => f.PickRandom(200, 201, 202, 204, 400, 404, 500))
					.RuleFor(x => x.ResponseMessage, (f, a) => {
						return a.ResponseStatusCode switch {
							200 => "OK",
							201 => "Created",
							202 => "Accepted",
							204 => "No Content",
							400 => "Bad Request",
							404 => "Not Found",
							500 => "Internal Server Error",
							_ => "Unknown"
						};
					})
					.RuleFor(x => x.StartedAt, f => f.Date.Past())
					.RuleFor(x => x.EndedAt, (f, a) => a.StartedAt.AddMilliseconds(f.Random.Int(100, 1000)));

				return f2.Generate(attemptCount);
			});
		}
	}
}
