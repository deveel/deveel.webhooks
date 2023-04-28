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

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	/// <summary>
	/// A service that handles a specific event and 
	/// transforms its data
	/// </summary>
	public interface IEventDataTransformer {
		/// <summary>
		/// Determines if the instance can handle
		/// the event given and transforms it.
		/// </summary>
		/// <param name="eventInfo">The information of the
		/// event to be handled.</param>
		/// <remarks>
		/// During the process of resolution of data creators,
		/// the system stops at the first that is capable of handling
		/// the event given: this means that if two instances of
		/// this contract are capable of handling the same event,
		/// only the first one is executed and the data used.
		/// </remarks>
		/// <returns>
		/// Returns <strong>true</strong> if the instance is
		/// capable of handling the data transformations for the
		/// event give, <strong>false</strong> otherwise.
		/// </returns>
		bool Handles(EventInfo eventInfo);

		/// <summary>
		/// Creates an object that is used to form
		/// the contents of a webhook.
		/// </summary>
		/// <param name="eventInfo">The information of the event
		/// that carries the base data to be transformed.</param>
		/// <param name="cancellationToken">A token used to synchronize
		/// the read operations.</param>
		/// <returns>
		/// Returns an object that is used to form the contents of
		/// a webhook delivered to subscribers.
		/// </returns>
		Task<object> CreateDataAsync(EventInfo eventInfo, CancellationToken cancellationToken);
	}
}
