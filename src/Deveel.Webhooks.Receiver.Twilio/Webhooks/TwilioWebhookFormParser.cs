using Deveel.Webhooks.Twilio;

using Microsoft.AspNetCore.Http;

namespace Deveel.Webhooks {
    class TwilioWebhookFormParser : IWebhookFormParser<TwilioWebhook> {
        public Task<TwilioWebhook> ParseWebhookAsync(IFormCollection form, CancellationToken cancellationToken) {
			try {
                var result = TwilioWebhookParser.FromForm(form);
                return Task.FromResult(result);
			} catch (WebhookParseException) {
				throw;
			} catch(Exception ex) {
                throw new WebhookParseException("Unable to parse the form request into a Twilio webhook", ex);
            }
        }
    }
}
