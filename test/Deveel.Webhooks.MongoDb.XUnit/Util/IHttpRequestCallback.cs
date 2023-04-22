using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Deveel.Util {
	interface IHttpRequestCallback {
		Task<HttpResponseMessage> RequestsAsync(HttpRequestMessage request, CancellationToken cancellationToken);
	}
}
