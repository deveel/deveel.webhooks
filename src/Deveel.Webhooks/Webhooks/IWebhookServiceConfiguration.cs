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
		/// Gets a serializer that supports the given format.
		/// </summary>
		/// <param name="format">The format of the webhook payload.</param>
		/// <returns>
		/// Returns an instance of the <see cref="IWebhookSerializer"/> that supports
		/// the provided <paramref name="format"/>.
		/// </returns>
		IWebhookSerializer GetSerializer(string format);

		/// <summary>
		/// Gets a signer for the given algorithm.
		/// </summary>
		/// <param name="algorithm">The signing algorithm to be used.</param>
		/// <returns>
		/// Returns an instance of <see cref="IWebhookSigner"/> that can be
		/// used to sign the webhooks through the <paramref name="algorithm"/>
		/// provided.
		/// </returns>
		IWebhookSigner GetSigner(string algorithm);

		/// <summary>
		/// Gets an evaluaor of filters that is supporting
		/// the given format
		/// </summary>
		/// <param name="filterFormat">The format of filters to be evaluated</param>
		/// <returns>
		/// Returns an instance of <see cref="IWebhookFilterEvaluator"/> that supports
		/// the <paramref name="filterFormat">format</paramref> given to evaluate
		/// some subscription filters.
		/// </returns>
		IWebhookFilterEvaluator GetFilterEvaluator(string filterFormat);
	}
}
