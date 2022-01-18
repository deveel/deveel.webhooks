﻿// Copyright 2022 Deveel
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
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides a contract to evaluate filters of webhook
	/// subscriptions
	/// </summary>
	public interface IWebhookFilterEvaluator {
		/// <summary>
		/// Gets the format of the field handled
		/// </summary>
		string Format { get; }

		/// <summary>
		/// Evaluates if a given aggregation of filters matches
		/// the webhook given
		/// </summary>
		/// <param name="filterRequest">A request of filtering, containing
		/// one or more filters.</param>
		/// <param name="webhook">The webhook argument of the filter.</param>
		/// <param name="cancellationToken"></param>
		/// <returns>
		/// Returns <strong>true</strong> if the given set of filters
		/// match the conditions given against the provided webhook instance,
		/// <strong>false</strong> otherwise.
		/// </returns>
		Task<bool> MatchesAsync(WebhookFilterRequest filterRequest, IWebhook webhook, CancellationToken cancellationToken);
	}
}
