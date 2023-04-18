using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace Deveel.Webhooks {
	public interface IWebhookRequestVerifier<TWebhook> {
		Task VerifyRequestAsync(HttpContext context);
	}
}
