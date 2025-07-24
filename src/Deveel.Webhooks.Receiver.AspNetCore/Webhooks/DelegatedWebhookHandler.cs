// Copyright 2022-2025 Antonello Provenzano
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
	class DelegatedWebhookHandler<TWebhook> : IWebhookHandlerInitialize<TWebhook> where TWebhook : class {
		private IServiceProvider? provider;
		private readonly Delegate callback;

		public DelegatedWebhookHandler(Delegate callback) {
			ValidateWebhookParameter(callback);

			this.callback = callback ?? throw new ArgumentNullException(nameof(callback));
		}

		public void Initialize(IServiceProvider serviceProvider) {
			provider = serviceProvider;
		}

		private static void ValidateWebhookParameter(Delegate callback) {
			var parameters = callback.Method.GetParameters();
			if (parameters.Length == 0 ||
				!typeof(TWebhook).IsAssignableFrom(parameters[0].ParameterType))
				throw new ArgumentException($"The callback delegate must have the first parameter of type {typeof(TWebhook)}");

			if (callback.Method.ReturnType != typeof(void) &&
				callback.Method.ReturnType != typeof(Task))
				throw new ArgumentException($"The callback delegate must return void or Task");
		}

		private bool IsAsync => callback.Method.ReturnType == typeof(Task);

		public Task HandleAsync(TWebhook webhook, CancellationToken cancellationToken = default) {
			if (provider == null)
				throw new InvalidOperationException("The handler has not been initialized");

			try {
				var parameters = callback.Method.GetParameters();
				var args = new object?[parameters.Length];
				args[0] = webhook;

				for (var i = 1; i < parameters.Length; i++) {
					var parameter = parameters[i];
					if (parameter.ParameterType == typeof(CancellationToken)) {
						args[i] = cancellationToken;
					} else {
						// TODO: handle optional and default parameters ...
						args[i] = provider.GetService(parameter.ParameterType);
					}
				}

				var result = callback.DynamicInvoke(args);
				if (result is Task task)
					return task;

				return Task.CompletedTask;
			} catch (Exception ex) {
				throw new WebhookReceiverException("Error while executing the delegated handler", ex);
			}
		}
	}
}
