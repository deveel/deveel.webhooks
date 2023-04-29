using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Deveel.Webhooks.Twilio;

namespace Deveel.Webhooks {
	[ExcludeFromCodeCoverage]
	public static class WebhookReceiverBuilderExtensions {
		public static WebhookReceiverBuilder<TwilioWebhook> HandleTwilio<THandler>(this WebhookReceiverBuilder<TwilioWebhook> builder)
			where THandler : class, IWebhookHandler<TwilioWebhook>
			=> builder.AddHandler<THandler>();

		public static WebhookReceiverBuilder<TwilioWebhook> HandleTwilio(this WebhookReceiverBuilder<TwilioWebhook> builder, Func<TwilioWebhook, Task> handler)
			=> builder.HandleAsync(handler);

		public static WebhookReceiverBuilder<TwilioWebhook> HandleTwilio<T1>(this WebhookReceiverBuilder<TwilioWebhook> builder, Func<TwilioWebhook, T1, Task> handler)
			=> builder.HandleAsync(handler);

		public static WebhookReceiverBuilder<TwilioWebhook> HandleTwilio<T1, T2>(this WebhookReceiverBuilder<TwilioWebhook> builder, Func<TwilioWebhook, T1, T2, Task> handler)
			=> builder.HandleAsync(handler);

		public static WebhookReceiverBuilder<TwilioWebhook> HandleTwilio<T1, T2, T3>(this WebhookReceiverBuilder<TwilioWebhook> builder, Func<TwilioWebhook, T1, T2, T3, Task> handler)
			=> builder.HandleAsync(handler);

		public static WebhookReceiverBuilder<TwilioWebhook> HandleTwilio(this WebhookReceiverBuilder<TwilioWebhook> builder, Action<TwilioWebhook> handler)
			=> builder.Handle(handler);

		public static WebhookReceiverBuilder<TwilioWebhook> HandleTwilio<T1>(this WebhookReceiverBuilder<TwilioWebhook> builder, Action<TwilioWebhook, T1> handler)
			=> builder.Handle(handler);

		public static WebhookReceiverBuilder<TwilioWebhook> HandleTwilio<T1, T2>(this WebhookReceiverBuilder<TwilioWebhook> builder, Action<TwilioWebhook, T1, T2> handler)
			=> builder.Handle(handler);

		public static WebhookReceiverBuilder<TwilioWebhook> HandleTwilio<T1, T2, T3>(this WebhookReceiverBuilder<TwilioWebhook> builder, Action<TwilioWebhook, T1, T2, T3> handler)
			=> builder.Handle(handler);
	}
}
