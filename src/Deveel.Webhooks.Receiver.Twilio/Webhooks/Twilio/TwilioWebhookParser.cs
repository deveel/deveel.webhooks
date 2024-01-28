// Copyright 2022-2024 Antonello Provenzano
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Globalization;

using Microsoft.AspNetCore.Http;

namespace Deveel.Webhooks.Twilio {
    static class TwilioWebhookParser {
        public static TwilioWebhook Parse(IFormCollection form) {

            try {
                var data = new TwilioWebhook();

                int? numMedia = null;
				int? numSegments = null;

                foreach (var kvp in form) {
                    switch (kvp.Key) {
                        case "ToCountry":
                            data.To.Country = kvp.Value;
                            break;
                        case "ToState":
                            data.To.State = kvp.Value;
                            break;
                        case "NumMedia":
                            numMedia = Int32.TryParse(kvp.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i) ? i : null;
                            break;
                        case "ToCity":
                            data.To.City = kvp.Value;
                            break;
                        case "FromZip":
                            data.From.Zip = kvp.Value;
                            break;
                        case "SmsSid":
                            data.SmsId = kvp.Value;
                            break;
                        case "FromState":
                            data.From.State = kvp.Value;
                            break;
                        case "SmsStatus":
						case "MessageStatus":
                            data.MessageStatus = ParseMessageStatus(kvp.Value);
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
							if (Int32.TryParse(kvp.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var ns))
								numSegments = ns;

							data.SegmentCount = numSegments;
                            break;
                        case "MessageSid":
						case "SmsMessageSid":
							data.MessageId = kvp.Value;
                            break;
                        case "AccountSid":
                            data.AccountId = kvp.Value;
                            break;
                        case "From":
                            data.From.PhoneNumber = kvp.Value;
                            break;
                        case "ApiVersion":
                            data.ApiVersion = kvp.Value;
                            break;
						case "ErrorCode":
							data.ErrorCode = kvp.Value;
							break;
						case "ErrorMessage":
							data.ErrorMessage = kvp.Value;
							break;
                        default:
                            // Ignore unknown form items
                            break;
                    }
                }

				if (numSegments.HasValue && numSegments.Value > 0) {
					data.Segments = new Segment[numSegments.Value];

					for (var i = 0; i < numSegments.Value; i++) {
						var segment = new Segment {
							Index = i
						};

						if (form.TryGetValue($"Body{i}", out var body))
							segment.Text = body;
						if (form.TryGetValue($"MessageSid{i}", out var messageSid))
							segment.MessageId = messageSid;

						data.Segments[i] = segment;
					}
				}

				if (numMedia.HasValue && numMedia.Value > 0) {
					data.Media = new MediaElement[numMedia.Value];
					for (var i = 0; i < numMedia.Value; i++) {
						var mediaElement = new MediaElement();

						if (form.TryGetValue($"MediaContentType{i}", out var mediaContentType))
							mediaElement.ContentType = mediaContentType;

						if (form.TryGetValue($"MediaUrl{i}", out var mediaUrl))
							mediaElement.Url = mediaUrl;

						if (form.TryGetValue($"MediaContentLength{i}", out var mediaContentLength) &&
							long.TryParse(mediaContentLength, out var cl))
							mediaElement.ContentLength = cl;

						data.Media[i] = mediaElement;
					}
				}

                return data;
            } catch (Exception ex) {

                throw new WebhookParseException("Unable to read the Twilio webhook", ex);
            }
        }

        private static MessageStatus ParseMessageStatus(string? value) {
            if (!Enum.TryParse<MessageStatus>(value, true, out var status))
                return MessageStatus.Unknown;

            return status;
        }
    }
}
