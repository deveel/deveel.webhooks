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
	/// Defines the contract for a resolver of a <see cref="IWebhookSubscription"/>
	/// that is specific to a given webhook type.
	/// </summary>
	/// <typeparam name="TWebhook">
	/// The type of the webhook that is scoped for the resolver.
	/// </typeparam>
	/// <remarks>
	/// Implementations of this version of the interface are 
	/// segregated to the scope of the webhook type.
	/// </remarks>
	/// <seealso cref="IWebhookSubscriptionResolver"/>
	public interface IWebhookSubscriptionResolver<TWebhook> : IWebhookSubscriptionResolver where TWebhook : class {
	}
}
