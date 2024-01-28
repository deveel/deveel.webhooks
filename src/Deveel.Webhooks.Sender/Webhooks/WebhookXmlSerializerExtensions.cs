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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	/// <summary>
	/// Extends the <see cref="IWebhookXmlSerializer{TWebhook}"/> interface
	/// to provide some helper methods to serialize a webhook.
	/// </summary>
	public static class WebhookXmlSerializerExtensions {
		/// <summary>
		/// Serializes the given <paramref name="webhook"/> to a XML string.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of the webhook to serialize.
		/// </typeparam>
		/// <param name="serializer">
		/// The serializer to use to serialize the webhook.
		/// </param>
		/// <param name="webhook">
		/// The instance of the webhook to serialize.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a string that represents the XML serialization of the webhook.
		/// </returns>
		/// <exception cref="WebhookSerializationException">
		/// Thrown if it was not possible to serialize the webhook to a XML string.
		/// </exception>
		public static async Task<string> SerializeToStringAsync<TWebhook>(this IWebhookXmlSerializer<TWebhook> serializer, TWebhook webhook, CancellationToken cancellationToken = default)
			where TWebhook : class {
			try {
				using var stream = new MemoryStream();
				await serializer.SerializeAsync(stream, webhook, cancellationToken);
				stream.Position = 0;

				using var reader = new StreamReader(stream, Encoding.UTF8);
				return await reader.ReadToEndAsync();
			} catch (WebhookSerializationException) {
				throw;
			} catch (Exception ex) {
				throw new WebhookSerializationException("It was not possible to serialize the webook to a XML string", ex);
			}
		}

		// TODO: add an extension to serialize to an object, to support filtering
	}
}
