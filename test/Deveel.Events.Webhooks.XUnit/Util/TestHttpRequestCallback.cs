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
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Deveel.Util {
	class TestHttpRequestCallback : IHttpRequestCallback {
		private readonly Func<HttpRequestMessage, HttpResponseMessage> func;

		public TestHttpRequestCallback(Func<HttpRequestMessage, HttpResponseMessage> func) {
			this.func = func;
		}

		public Task<HttpResponseMessage> RequestsAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
			var response = func(request);
			return Task.FromResult(response);
		}

	}

	class TestHttpRequestAsyncCallback : IHttpRequestCallback {
		private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>>? func;
		private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>? cancellableFunc;

		public TestHttpRequestAsyncCallback(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> func) {
			cancellableFunc = func;
		}

		public TestHttpRequestAsyncCallback(Func<HttpRequestMessage, Task<HttpResponseMessage>> func) {
			this.func = func;
		}

		public Task<HttpResponseMessage> RequestsAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
			if (func != null)
				return func(request);
			if (cancellableFunc != null)
				return cancellableFunc(request, cancellationToken);

			throw new InvalidOperationException();
		}
	}
}
