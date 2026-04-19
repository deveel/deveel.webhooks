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

using System.Net.Http;

namespace Deveel.Webhooks {
	public static class WebhookDestinationAndVerificationTests {
		[Fact]
		public static void AddQueryParameter_WithInvalidName_ThrowsArgumentException() {
			var uri = new Uri("https://example.test/webhooks");

			Assert.Throws<ArgumentException>(() => uri.AddQueryParameter(" ", "value"));
		}

		[Fact]
		public static void AddQueryParameter_AddsOrOverridesValue() {
			var uri = new Uri("https://example.test/webhooks?token=old");

			var result = uri.AddQueryParameter("token", "new");

			Assert.Equal("https://example.test/webhooks?token=new", result.ToString());
		}

		[Fact]
		public static void WebhookDestination_WithRelativeUri_ThrowsArgumentException() {
			var relative = new Uri("/webhooks", UriKind.Relative);

			Assert.Throws<ArgumentException>(() => new WebhookDestination(relative));
		}

		[Fact]
		public static void WebhookDestination_WithEmptyString_ThrowsArgumentException() {
			Assert.Throws<ArgumentException>(() => new WebhookDestination(" "));
		}

		[Fact]
		public static void WebhookDestination_WithSignature_SetsSignToTrue() {
			var destination = new WebhookDestination("https://example.test/webhooks");

			destination.WithSignature(new WebhookDestinationSignatureOptions {
				Secret = "secret"
			});

			Assert.True(destination.Sign);
			Assert.Equal("secret", destination.Signature?.Secret);
		}

		[Fact]
		public static void WebhookDestination_Merge_UsesDefaultAndDestinationHeaders() {
			var destination = new WebhookDestination("https://example.test/webhooks") {
				Headers = new Dictionary<string, string> {
					["x-default"] = "override",
					["x-custom"] = "custom"
				},
				Format = null,
				Retry = null,
				Signature = null
			};

			var options = new WebhookSenderOptions<TestWebhook> {
				DefaultFormat = WebhookFormat.Xml,
				DefaultHeaders = new Dictionary<string, string> {
					["x-default"] = "default",
					["x-other"] = "other"
				},
				Retry = new WebhookRetryOptions {
					MaxRetries = 3
				}
			};

			var merged = destination.Merge(options);

			Assert.Equal(WebhookFormat.Xml, merged.Format);
			Assert.Equal("override", merged.Headers!["x-default"]);
			Assert.Equal("other", merged.Headers["x-other"]);
			Assert.Equal("custom", merged.Headers["x-custom"]);
			Assert.Equal(3, merged.Retry?.MaxRetries);
			Assert.NotNull(merged.Signature);
		}

		[Fact]
		public static void TryGetVerifyToken_ReturnsFalse_WhenTokenMissing() {
			var verifier = CreateVerifier();

			var ok = verifier.TryGetVerifyTokenProxy(new Dictionary<string, object>(), out var token);

			Assert.False(ok);
			Assert.Null(token);
		}

		[Fact]
		public static void TryGetVerifyToken_ConvertsTokenValueToString() {
			var verifier = CreateVerifier();
			var parameters = new Dictionary<string, object> {
				["token"] = 12345
			};

			var ok = verifier.TryGetVerifyTokenProxy(parameters, out var token);

			Assert.True(ok);
			Assert.Equal("12345", token);
		}

		[Fact]
		public static void CreateRequest_WithTokenInHeader_SetsHeaderOnly() {
			var verifier = CreateVerifier(options => {
				options.Verification.TokenLocation = VerificationTokenLocation.Header;
				options.Verification.Challenge = false;
			});

			var request = verifier.CreateRequestProxy(new Uri("https://example.test/verify"), "abc", null);

			Assert.Equal("abc", request.Headers.GetValues(WebhookSenderDefaults.VerifyTokenHeaderName).Single());
			Assert.Equal("https://example.test/verify", request.RequestUri!.ToString());
		}

		[Fact]
		public static void AddChallenge_WithoutParameter_ThrowsNotSupportedException() {
			var verifier = CreateVerifier(options => options.Verification.ChallengeQueryParameter = null);
			var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test/verify");

			Assert.Throws<NotSupportedException>(() => verifier.AddChallengeProxy(request, "999"));
		}

		private static TestDestinationVerifier CreateVerifier(Action<WebhookSenderOptions<TestWebhook>>? configure = null) {
			var options = new WebhookSenderOptions<TestWebhook>();
			configure?.Invoke(options);
			return new TestDestinationVerifier(options);
		}

		private sealed class TestDestinationVerifier : WebhookDestinationVerifier<TestWebhook> {
			public TestDestinationVerifier(WebhookSenderOptions<TestWebhook> options)
				: base(options, new StubHttpClientFactory()) {
			}

			public bool TryGetVerifyTokenProxy(IDictionary<string, object> parameters, out string? token)
				=> TryGetVerifyToken(parameters, out token);

			public HttpRequestMessage CreateRequestProxy(Uri verificationUrl, string token, string? challenge)
				=> CreateRequest(verificationUrl, token, challenge);

			public void AddChallengeProxy(HttpRequestMessage request, string challenge)
				=> AddChallenge(request, challenge);
		}

		private sealed class StubHttpClientFactory : IHttpClientFactory {
			public HttpClient CreateClient(string name) => new();
		}

		private sealed class TestWebhook {
		}
	}
}
