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
using System.Text;
using System.Threading.Tasks;

namespace Deveel {
	class MockHttpClientFactory : IHttpClientFactory, IDisposable {
		private Dictionary<string, HttpClient> clients = new Dictionary<string, HttpClient>();

		public MockHttpClientFactory(string name, HttpClient client) {
			RegisterClient(name, client);
		}

		public MockHttpClientFactory() {
		}

		public HttpClient CreateClient(string name) {
			if (!clients.TryGetValue(name, out var client))
				throw new ArgumentException($"No client with name '{name}' was registered.");

			return client;
		}

		public void RegisterClient(string name, HttpClient client) {
			clients.Add(name, client);
		}

		public void Dispose() {
			foreach (var client in clients.Values) {
				client.Dispose();
			}
		}
	}
}
