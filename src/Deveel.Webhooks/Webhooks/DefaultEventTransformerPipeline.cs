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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	/// <summary>
	/// A default implementation of <see cref="IEventTransformerPipeline"/> that
	/// will iterate through all the registered <see cref="IEventDataTransformer"/>
	/// instances and transform an input event.
	/// </summary>
	public class DefaultEventTransformerPipeline : IEventTransformerPipeline {
		private readonly IEnumerable<IEventDataTransformer>? dataTransformers;

		/// <summary>
		/// Constructs the pipeline with the given transformers.
		/// </summary>
		/// <param name="dataTransformers">
		/// The collection of the registered transformers to use to transform the event.
		/// </param>
		public DefaultEventTransformerPipeline(IEnumerable<IEventDataTransformer>? dataTransformers) {
			this.dataTransformers = dataTransformers;
		}

		/// <inheritdoc/>
		public async Task<EventInfo> TransformAsync(EventInfo eventInfo, CancellationToken cancellationToken) {
			if (dataTransformers != null) {
				try {
					foreach (var dataTransformer in dataTransformers) {
						if (dataTransformer.Handles(eventInfo)) {
							var data = await dataTransformer.CreateDataAsync(eventInfo, cancellationToken);
							eventInfo = eventInfo.WithData(data);
						}
					}
				} catch (Exception ex) {
					throw new WebhookException("Unable to factory the data for the event", ex);
				}
			}

			return eventInfo;
		}
	}
}
