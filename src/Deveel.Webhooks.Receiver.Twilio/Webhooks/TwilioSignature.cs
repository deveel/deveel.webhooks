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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace Deveel.Webhooks {
	/// <summary>
	/// A utility class to create a Twilio signature for a request.
	/// </summary>
	public static class TwilioSignature {
		/// <summary>
		/// Creates a Twilio signature for the specified <paramref name="request"/>
		/// </summary>
		/// <param name="request">
		/// The <see cref="HttpRequest"/> to create the signature for.
		/// </param>
		/// <param name="authToken">
		/// A string containing the Twilio auth token, used as a key to create the signature.
		/// </param>
		/// <returns></returns>
		public static string Create(HttpRequest request, string authToken) {
			// Get the URL
			string url = $"{request.Scheme}://{request.Host + request.Path}";

			// Get the post parameters
			var postParams = new Dictionary<string, string?>();
			foreach (string key in request.Form.Keys) {
				postParams.Add(key, request.Form[key]);
			}

			// Sort the post parameters
			var sortedParams = postParams.OrderBy(x => x.Key).ToDictionary(x => x.Key, y => y.Value);

			// Concatenate the URL and post parameters
			string postData = "";
			foreach (var param in sortedParams) {
				postData += param.Key + param.Value;
			}

			string data = url + postData;
			byte[] keyBytes = Encoding.UTF8.GetBytes(authToken);
			byte[] messageBytes = Encoding.UTF8.GetBytes(data);
			HMACSHA1 hmac = new HMACSHA1(keyBytes);
			byte[] signatureBytes = hmac.ComputeHash(messageBytes);

			return Convert.ToBase64String(signatureBytes);
		}
	}
}
