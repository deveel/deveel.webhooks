using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Deveel.Facebook;

using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	class ConfigureWebhookHandlingOptions : IPostConfigureOptions<WebhookHandlingOptions> {
		public void PostConfigure(string name, WebhookHandlingOptions options) {
			if (String.Equals(name, nameof(FacebookWebhook))) {
				options.ResponseStatusCode = 200;
				options.InvalidStatusCode = 400;
			}
		}
	}
}
