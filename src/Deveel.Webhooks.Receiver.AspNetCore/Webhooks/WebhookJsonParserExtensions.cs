// Copyright 2022-2023 Deveel
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

using System.Text;

namespace Deveel.Webhooks {
    /// <summary>
    /// Extends the <see cref="IWebhookJsonParser{TWebhook}"/> with
    /// methods for the parsing of a webhooks.
    /// </summary>
    public static class WebhookJsonParserExtensions {
        /// <summary>
        /// Parses a webhook from the given <paramref name="json"/> string.
        /// </summary>
        /// <typeparam name="TWebhook">The type of the webhook to be parsed</typeparam>
        /// <param name="parser">The instance of the <see cref="IWebhookJsonParser{TWebhook}"/> to extend</param>
        /// <param name="json">The UTF-8 encoded JSON-formatted string to be parsed</param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// Returns a <see cref="Task{TResult}"/> that resolves to the parsed webhook
        /// </returns>
        /// <exception cref="WebhookParseException">
        /// Thrown if any error occurs while parsing the webhook
        /// </exception>
        public static async Task<TWebhook?> ParseWebhookAsync<TWebhook>(this IWebhookJsonParser<TWebhook> parser, string? json, CancellationToken cancellationToken = default)
            where TWebhook : class {
			try {
				if (string.IsNullOrWhiteSpace(json))
					return null;

				using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
				return await parser.ParseWebhookAsync(stream, cancellationToken);
			} catch (WebhookParseException) {

				throw;
			} catch(Exception ex) {
				throw new WebhookParseException("Unable to parse the JSON string into a webhook", ex);
			}
        }

		public static async Task<IList<TWebhook>> ParseWebhookArrayAsync<TWebhook>(this IWebhookJsonArrayParser<TWebhook> parser, string? json, CancellationToken cancellationToken = default)
			where TWebhook : class {
			try {
				if (string.IsNullOrWhiteSpace(json))
					return new List<TWebhook>();

				using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
				return await parser.ParseWebhookArrayAsync(stream, cancellationToken);
			} catch (WebhookParseException) {

				throw;
			} catch(Exception ex) {
				throw new WebhookParseException("Unable to parse the JSON string into a webhook list", ex);
			}
		}
    }
}
