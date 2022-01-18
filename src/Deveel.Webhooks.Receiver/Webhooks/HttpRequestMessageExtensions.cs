// Copyright 2022 Deveel
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
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using Newtonsoft.Json.Linq;

namespace Deveel.Webhooks {
	public static class HttpRequestMessageExtensions {
		public static Task<T> GetWebhookAsync<T>(this HttpRequestMessage request, Action<JObject, T> afterRead, CancellationToken cancellationToken)
			where T : class
			=> request.GetWebhookAsync(new WebhookReceiveOptions(), afterRead, cancellationToken);

		public static Task<T> GetWebhookAsync<T>(this HttpRequestMessage request, WebhookReceiveOptions options, CancellationToken cancellationToken)
			where T : class
			=> request.GetWebhookAsync<T>(options, null, cancellationToken);

		public static Task<T> GetWebhookAsync<T>(this HttpRequestMessage request, WebhookReceiveOptions options)
			where T : class
			=> request.GetWebhookAsync<T>(options, default);

		public static Task<T> GetWebhookAsync<T>(this HttpRequestMessage request, CancellationToken cancellationToken)
			where T : class
			=> request.GetWebhookAsync<T>(new WebhookReceiveOptions(), cancellationToken);

		public static Task<T> GetWebhookAsync<T>(this HttpRequestMessage request)
			where T : class
			=> request.GetWebhookAsync<T>(default(CancellationToken));

		public static async Task<T> GetWebhookAsync<T>(this HttpRequestMessage request, WebhookReceiveOptions options, Action<JObject, T> afterRead, CancellationToken cancellationToken)
			where T : class {
			if (options != null && options.ValidateSignature) {
				var content = await request.Content.ReadAsStringAsync();

				if (!request.IsSignatureValid(content, options))
					throw new ArgumentException("The signature of the webhook is invalid");

				return await WebhookJsonParser.ParseAsync(content, options.JsonSerializerSettings, afterRead, cancellationToken);
			}

			return await request.Content.ReadAsObjectAsync(options?.JsonSerializerSettings, afterRead, cancellationToken);
		}

		private static bool IsSignatureValid(this HttpRequestMessage request, string content, WebhookReceiveOptions options) {
			string signature;
			string algorithm = null;

			switch (options.SignatureLocation) {
				case WebhookSignatureLocation.Header:
					if (!request.Headers.TryGetValues(options.SignatureHeaderName, out var headerValue))
						return false;

					signature = headerValue.SingleOrDefault();

					if (!string.IsNullOrEmpty(signature)) {
						if (signature.StartsWith("sha256=")) {
							signature = signature.Substring("sha256=".Length - 1);
							algorithm = "sha256";
						}
					}

					break;
				case WebhookSignatureLocation.QueryString:
					if (string.IsNullOrWhiteSpace(request.RequestUri.Query))
						return false;

					var queryString = HttpUtility.ParseQueryString(request.RequestUri.Query);
					signature = queryString[options.SignatureQueryStringKey];
					algorithm = queryString["sig_alg"];

					break;
				default:
					// should never happen
					throw new NotSupportedException();
			}

			if (string.IsNullOrWhiteSpace(signature))
				return false;

			return WebhookSignatureValidator.IsValid(algorithm, content, options.Secret, signature);
		}

	}
}
