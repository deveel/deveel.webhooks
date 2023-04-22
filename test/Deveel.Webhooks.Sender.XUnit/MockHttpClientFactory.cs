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
