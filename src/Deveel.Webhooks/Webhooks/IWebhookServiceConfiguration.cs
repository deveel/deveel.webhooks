// Copyright 2022 Deveel
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

using System;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides the functions to obtain the configurations of the webhook service
	/// </summary>
	public interface IWebhookServiceConfiguration {
		/// <summary>
		/// Gets a set of options to configure the delivery behavior.
		/// </summary>
		WebhookDeliveryOptions DeliveryOptions { get; }

		/// <summary>
		/// Gets a collection of the registered webhook serializers.
		/// </summary>
		IWebhookSerializerCollection Serializers { get; }

		/// <summary>
		/// Gets a collection the registered webhook signers.
		/// </summary>
		IWebhookSignerCollection Signers { get; }

		/// <summary>
		/// Gets a collection of the data factories registered in the service
		/// </summary>
		IWebhookDataFactoryCollection DataFactories { get; }

		/// <summary>
		/// Gets all the registered filter evaluating engines.
		/// </summary>
		IWebhookFilterEvaluatorCollection FilterEvaluators { get; }
	}
}
