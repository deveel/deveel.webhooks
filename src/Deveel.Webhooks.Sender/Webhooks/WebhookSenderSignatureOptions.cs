using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public class WebhookSenderSignatureOptions {
		public WebhookSignatureLocation? Location { get; set; }

		public string? AlgorithmQueryParameter { get; set; }

		public string? HeaderName { get; set; }

		public string? Algorithm { get; set; }

		public string? QueryParameter { get; set; }
	}
}
