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

using System.Text;

namespace Deveel.Webhooks {
	/// <summary>
	/// Extends the <see cref="IWebhookXmlParser{TWebhook}"/> interface to provide
	/// methods for the deserialization of a webhook from a XML string.
	/// </summary>
    public static class WebhookXmlParserExtensions {
		/// <summary>
		/// Parses the given XML string into a webhook object.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of the webhook to be parsed.
		/// </typeparam>
		/// <param name="parser">
		/// The instance of the parser to be used to parse the XML string.
		/// </param>
		/// <param name="xmlBody">
		/// The string that contains the XML to be parsed.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns an instance of the webhook object parsed from the XML string,
		/// or <c>null</c> if the string is <c>null</c> or empty.
		/// </returns>
		/// <exception cref="WebhookParseException">
		/// Thrown when the XML string cannot be parsed into a webhook object.
		/// </exception>
		public static async Task<TWebhook?> ParseWebhookAsync<TWebhook>(this IWebhookXmlParser<TWebhook> parser, string? xmlBody, CancellationToken cancellationToken = default)
			where TWebhook : class {
			try {
				if (string.IsNullOrWhiteSpace(xmlBody))
					return null;

				using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlBody));
				return await parser.ParseWebhookAsync(stream, cancellationToken);
			} catch(WebhookParseException) {
				throw;
            } catch (Exception ex) {
				throw new WebhookParseException("Unable to parse the XML string into a webhook", ex);
			}
		}
	}
}
