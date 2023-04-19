using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public class WebhookRetryOptions {
		public int? MaxRetries { get; set; } = 3;

		public TimeSpan? MaxDelay { get; set; } = TimeSpan.FromMilliseconds(500);

		public TimeSpan? TimeOut { get; set; } = TimeSpan.FromSeconds(10);
	}
}
