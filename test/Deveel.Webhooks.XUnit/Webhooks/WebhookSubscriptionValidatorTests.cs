// Copyright 2022-2025 Antonello Provenzano
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

using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Deveel.Webhooks {
	public static class WebhookSubscriptionValidatorTests {
		[Fact]
		public static async Task ValidateAsync_WithCancelledToken_ThrowsOperationCanceledException() {
			var validator = new WebhookSubscriptionValidator<TestWebhookSubscription, string>();
			using var cts = new CancellationTokenSource();
			cts.Cancel();

			await Assert.ThrowsAsync<OperationCanceledException>(async () =>
				await CollectAsync(validator.ValidateAsync(null!, NewSubscription("https://example.test/webhooks"), cts.Token)));
		}

		[Fact]
		public static async Task ValidateAsync_WithMissingDestinationUrl_ReturnsValidationError() {
			var validator = new WebhookSubscriptionValidator<TestWebhookSubscription, string>();
			var subscription = NewSubscription(" ");

			var results = await CollectAsync(validator.ValidateAsync(null!, subscription, CancellationToken.None));

			Assert.Single(results);
			Assert.Contains("missing", results[0].ErrorMessage, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public static async Task ValidateAsync_WithInvalidDestinationUrlFormat_ReturnsValidationError() {
			var validator = new WebhookSubscriptionValidator<TestWebhookSubscription, string>();
			var subscription = NewSubscription("not-a-url");

			var results = await CollectAsync(validator.ValidateAsync(null!, subscription, CancellationToken.None));

			Assert.Single(results);
			Assert.Contains("invalid", results[0].ErrorMessage, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public static async Task ValidateAsync_WithUnsupportedScheme_ReturnsValidationError() {
			var validator = new WebhookSubscriptionValidator<TestWebhookSubscription, string>();
			var subscription = NewSubscription("ftp://example.test/webhooks");

			var results = await CollectAsync(validator.ValidateAsync(null!, subscription, CancellationToken.None));

			Assert.Single(results);
			Assert.Contains("not supported", results[0].ErrorMessage, StringComparison.OrdinalIgnoreCase);
		}

		[Theory]
		[InlineData("http://example.test/webhooks")]
		[InlineData("https://example.test/webhooks")]
		public static async Task ValidateAsync_WithSupportedHttpScheme_ReturnsNoErrors(string url) {
			var validator = new WebhookSubscriptionValidator<TestWebhookSubscription, string>();
			var subscription = NewSubscription(url);

			var results = await CollectAsync(validator.ValidateAsync(null!, subscription, CancellationToken.None));

			Assert.Empty(results);
		}

		private static TestWebhookSubscription NewSubscription(string destinationUrl) {
			return new TestWebhookSubscription {
				SubscriptionId = "sub-1",
				TenantId = "tenant-1",
				Name = "test-subscription",
				EventTypes = new[] { "created" },
				DestinationUrl = destinationUrl,
				Filters = new List<WebhookFilter>(),
				Headers = new Dictionary<string, string>(),
				Properties = new Dictionary<string, object>(),
				Status = WebhookSubscriptionStatus.Active
			};
		}

		private static async Task<List<ValidationResult>> CollectAsync(IAsyncEnumerable<ValidationResult> source) {
			var results = new List<ValidationResult>();
			await foreach (var result in source) {
				results.Add(result);
			}

			return results;
		}
	}
}
