using Microsoft.AspNetCore.Http;

namespace Deveel.Webhooks.Twilio {
    public static class TwilioWebhookParser {
        public static TwilioWebhook FromForm(IFormCollection form) {

            try {
                var data = new TwilioWebhook();

                int? numMedia = null;

                foreach (var kvp in form) {
                    switch (kvp.Key) {
                        case "ToCountry":
                            data.To.Country = kvp.Value;
                            break;
                        case "ToState":
                            data.To.State = kvp.Value;
                            break;
                        case "SmsMessageSid":
                            data.SmsMessageSid = kvp.Value;
                            break;
                        case "NumMedia":
                            numMedia = int.Parse(kvp.Value);
                            break;
                        case "ToCity":
                            data.To.City = kvp.Value;
                            break;
                        case "FromZip":
                            data.From.Zip = kvp.Value;
                            break;
                        case "SmsSid":
                            data.SmsSid = kvp.Value;
                            break;
                        case "FromState":
                            data.From.State = kvp.Value;
                            break;
                        case "SmsStatus":
                            data.SmsStatus = ParseSmsStatus(kvp.Value);
                            break;
                        case "FromCity":
                            data.From.City = kvp.Value;
                            break;
                        case "Body":
                            data.Body = kvp.Value;
                            break;
                        case "FromCountry":
                            data.From.Country = kvp.Value;
                            break;
                        case "To":
                            data.To.PhoneNumber = kvp.Value;
                            break;
                        case "ToZip":
                            data.To.Zip = kvp.Value;
                            break;
                        case "NumSegments":
                            data.NumSegments = int.Parse(kvp.Value);
                            break;
                        case "MessageSid":
                            data.MessageSid = kvp.Value;
                            break;
                        case "AccountSid":
                            data.AccountSid = kvp.Value;
                            break;
                        case "From":
                            data.From.PhoneNumber = kvp.Value;
                            break;
                        case "ApiVersion":
                            data.ApiVersion = kvp.Value;
                            break;
                        default:
                            // Ignore unknown form items
                            break;
                    }
                }

                return data;
            } catch (Exception ex) {

                throw new WebhookParseException("Unable to read the Twilio webhook", ex);
            }
        }

        private static SmsStatus ParseSmsStatus(string value) {
            if (!Enum.TryParse<SmsStatus>(value, true, out var status))
                return SmsStatus.Unknown;

            return status;
        }
    }
}
