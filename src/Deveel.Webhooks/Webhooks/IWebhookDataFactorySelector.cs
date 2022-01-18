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
using System.Collections.Generic;
using System.Text;

namespace Deveel.Webhooks {
	/// <summary>
	/// A service that resolves the first <see cref="IWebhookDataFactory"/>
	/// handling an event given.
	/// </summary>
	public interface IWebhookDataFactorySelector {
		/// <summary>
		/// Resolves the first <see cref="IWebhookDataFactorySelector">data factory</see>
		/// capable of handling the given event.
		/// </summary>
		/// <param name="eventInfo">The information of the event
		/// that should be handled.</param>
		/// <returns>
		/// Returns a single <see cref="IWebhookDataFactory"/> instance capable
		/// of handling the given event, or <strong>null</strong> if
		/// none of the registered factories was able to handle that event.
		/// </returns>
		IWebhookDataFactory GetDataFactory(EventInfo eventInfo);
	}
}
