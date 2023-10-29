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

namespace Deveel.Webhooks {
	/// <summary>
	/// Extensions for the <see cref="WebhookNotifierBuilder{TWebhook}"/>
	/// to add the support for the dynamic LINQ filter evaluator.
	/// </summary>
	public static class WebhookNotifierBuilderExtensions {
		/// <summary>
		/// Registers a service that uses the dynamic LINQ
		/// expressions to evaluate the filter expression.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of the webhook to be used.
		/// </typeparam>
		/// <param name="builder">
		/// The instance of the builder to register the service.
		/// </param>
		/// <returns>
		/// Returns the same instance of the builder for chaining calls.
		/// </returns>
		public static WebhookNotifierBuilder<TWebhook> UseLinqFilter<TWebhook>(this WebhookNotifierBuilder<TWebhook> builder)
			where TWebhook : class 
			=> builder.AddFilterEvaluator<LinqWebhookFilterEvaluator<TWebhook>>();
	}
}
