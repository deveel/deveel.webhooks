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
using System.Collections.ObjectModel;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	public class WebhookServiceConfiguration : IWebhookServiceConfiguration {
		private readonly IEnumerable<IWebhookFilterEvaluator> filterEvaluators;
		private readonly IEnumerable<IWebhookDataFactory> dataFactories;
		private readonly IEnumerable<IWebhookSigner> signers;

		public WebhookServiceConfiguration(
			IEnumerable<IWebhookFilterEvaluator> filterEvaluators, 
			IEnumerable<IWebhookDataFactory> dataFactories, 
			IEnumerable<IWebhookSigner> signers) {
			this.filterEvaluators = filterEvaluators;
			this.dataFactories = dataFactories;
			this.signers = signers;
		}

		public IWebhookDataFactoryCollection DataFactories => new DataFactoryCollection(dataFactories);

		public IWebhookFilterEvaluatorCollection FilterEvaluators => new FilterEvaluatorCollection(filterEvaluators);

		#region DataFactoryCollection

		class DataFactoryCollection : ReadOnlyCollection<IWebhookDataFactory>, IWebhookDataFactoryCollection {
			public DataFactoryCollection(IEnumerable<IWebhookDataFactory> list) 
				: base(list?.ToList() ?? new List<IWebhookDataFactory>()) {
			}

			public IWebhookDataFactory Find(EventInfo eventInfo) 
				=> base.Items.FirstOrDefault(x => x.Handles(eventInfo));
		}

		#endregion

		#region FilterEvaluatorCollection

		class FilterEvaluatorCollection : ReadOnlyCollection<IWebhookFilterEvaluator>, IWebhookFilterEvaluatorCollection {
			public FilterEvaluatorCollection(IEnumerable<IWebhookFilterEvaluator> list) 
				: base(list?.ToList() ?? new List<IWebhookFilterEvaluator>()) {
			}

			public IWebhookFilterEvaluator this[string format] {
				get {
					if (String.IsNullOrWhiteSpace(format))
						throw new ArgumentNullException(nameof(format));

					return base.Items.Where(x => x.Format == format)
						.FirstOrDefault();
				}
			}

			// TODO: retrieve a configuration that specifies the default filtering engine...
			public IWebhookFilterEvaluator DefaultEvaluator => null;

			public bool Contains(string format) => Items.Where(x => x.Format == format).Any();

			public int IndexOf(string format) {
				for (int i = 0; i < Count; i++) {
					if (Items[i].Format == format)
						return i;
				}

				return -1;
			}
		}

		#endregion
	}
}
