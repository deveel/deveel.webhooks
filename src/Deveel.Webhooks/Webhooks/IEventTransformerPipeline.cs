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
	/// Provides a pipeline mechanism to transform the events
	/// incoming to a notifier, before forming a webhook to send.
	/// </summary>
	public interface IEventTransformerPipeline {
		/// <summary>
		/// Transforms the given event into a new one, that will be
		/// used to create the webhook to send.
		/// </summary>
		/// <param name="eventInfo">
		/// The original event to transform.
		/// </param>
		/// <param name="cancellationToken">
		/// A token used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns an instance of <see cref="EventInfo"/> that represents
		/// the final version of an event to use to create the webhook.
		/// </returns>
		Task<EventInfo> TransformAsync(EventInfo eventInfo, CancellationToken cancellationToken);
	}
}
