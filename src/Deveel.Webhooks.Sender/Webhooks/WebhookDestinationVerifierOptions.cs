using System;

namespace Deveel.Webhooks {
    public class WebhookDestinationVerifierOptions {        
        public TimeSpan? Timeout { get; set; }
        
        public TimeSpan? RetryTimeout { get; set; }
        
        public int? RetryCount { get; set; }

        public string HttpMethod { get; set; } = "GET";

        public string TokenQueryParameter { get; set; } = "token";

        public string TokenHeaderName { get; set; } = "X-Verify-Token";

        public VerificationTokenLocation TokenLocation { get; set; }
    }
}
