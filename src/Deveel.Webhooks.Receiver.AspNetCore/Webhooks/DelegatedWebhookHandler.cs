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
