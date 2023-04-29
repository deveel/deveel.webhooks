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

using System.Diagnostics.CodeAnalysis;

using Deveel.Webhooks.SendGrid;

using Microsoft.AspNetCore.Builder;

namespace Deveel.Webhooks {
	[ExcludeFromCodeCoverage]
	public static class ApplicationBuilderExtensions {
		public static IApplicationBuilder MapSendGridWebhook(this IApplicationBuilder app, string path = "/webhook/sendgrid")
			=> app.MapWebhook<SendGridWebhook>(path, new WebhookHandlingOptions {
				InvalidStatusCode = 400,
				// TODO: configure the options
			});

		public static IApplicationBuilder MapSendGridWebhook(this IApplicationBuilder app, string path, Func<SendGridWebhook, Task> handler)
			=> app.MapWebhook<SendGridWebhook>(path, handler);

		public static IApplicationBuilder MapSendGridWebhook<T1>(this IApplicationBuilder app, string path, Func<SendGridWebhook, T1, Task> handler)
			=> app.MapWebhook<SendGridWebhook, T1>(path, handler);

		public static IApplicationBuilder MapSendGridWebhook<T1, T2>(this IApplicationBuilder app, string path, Func<SendGridWebhook, T1, T2, Task> handler)
			=> app.MapWebhook<SendGridWebhook, T1, T2>(path, handler);

		public static IApplicationBuilder MapSendGridWebhook<T1, T2, T3>(this IApplicationBuilder app, string path, Func<SendGridWebhook, T1, T2, T3, Task> handler)
			=> app.MapWebhook<SendGridWebhook, T1, T2, T3>(path, handler);

		public static IApplicationBuilder MapSendGridWebhook(this IApplicationBuilder app, string path, Action<SendGridWebhook> handler)
			=> app.MapWebhook<SendGridWebhook>(path, handler);

		public static IApplicationBuilder MapSendGridWebhook<T1>(this IApplicationBuilder app, string path, Action<SendGridWebhook, T1> handler)
			=> app.MapWebhook<SendGridWebhook, T1>(path, handler);

		public static IApplicationBuilder MapSendGridWebhook<T1, T2>(this IApplicationBuilder app, string path, Action<SendGridWebhook, T1, T2> handler)
			=> app.MapWebhook<SendGridWebhook, T1, T2>(path, handler);

		public static IApplicationBuilder MapSendGridWebhook<T1, T2, T3>(this IApplicationBuilder app, string path, Action<SendGridWebhook, T1, T2, T3> handler)
			=> app.MapWebhook<SendGridWebhook, T1, T2, T3>(path, handler);

		public static IApplicationBuilder MapSendGridEmail(this IApplicationBuilder app, string path = "/email/sendgrid")
			=> app.MapWebhook<SendGridEmail>(path, new WebhookHandlingOptions {
				InvalidStatusCode = 400,
				ResponseStatusCode = 201,
			});

		public static IApplicationBuilder MapSendGridEmail(this IApplicationBuilder app, string path, Func<SendGridEmail, Task> handler)
			=> app.MapWebhook<SendGridEmail>(path, handler);

		public static IApplicationBuilder MapSendGridEmail<T1>(this IApplicationBuilder app, string path, Func<SendGridEmail, T1, Task> handler)
			=> app.MapWebhook<SendGridEmail, T1>(path, handler);

		public static IApplicationBuilder MapSendGridEmail<T1, T2>(this IApplicationBuilder app, string path, Func<SendGridEmail, T1, T2, Task> handler)
			=> app.MapWebhook<SendGridEmail, T1, T2>(path, handler);

		public static IApplicationBuilder MapSendGridEmail<T1, T2, T3>(this IApplicationBuilder app, string path, Func<SendGridEmail, T1, T2, T3, Task> handler)
			=> app.MapWebhook<SendGridEmail, T1, T2, T3>(path, handler);
	}
}
