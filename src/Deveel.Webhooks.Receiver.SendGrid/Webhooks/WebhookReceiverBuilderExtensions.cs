using System.Diagnostics.CodeAnalysis;

using Deveel.Webhooks.SendGrid;

namespace Deveel.Webhooks {
	[ExcludeFromCodeCoverage]
	public static class WebhookReceiverBuilderExtensions {
		public static WebhookReceiverBuilder<SendGridWebhook> HandleSendGridWebhook<THandler>(this WebhookReceiverBuilder<SendGridWebhook> builder)
			where THandler : class, IWebhookHandler<SendGridWebhook>
			=> builder.AddHandler<THandler>();

		public static WebhookReceiverBuilder<SendGridWebhook> HandleSendGrid(this WebhookReceiverBuilder<SendGridWebhook> builder, Func<SendGridWebhook, Task> handler)
			=> builder.HandleAsync(handler);

		public static WebhookReceiverBuilder<SendGridWebhook> HandleSendGrid<T1>(this WebhookReceiverBuilder<SendGridWebhook> builder, Func<SendGridWebhook, T1, Task> handler)
			=> builder.HandleAsync(handler);

		public static WebhookReceiverBuilder<SendGridWebhook> HandleSendGrid<T1, T2>(this WebhookReceiverBuilder<SendGridWebhook> builder, Func<SendGridWebhook, T1, T2, Task> handler)
			=> builder.HandleAsync(handler);

		public static WebhookReceiverBuilder<SendGridWebhook> HandleSendGrid<T1, T2, T3>(this WebhookReceiverBuilder<SendGridWebhook> builder, Func<SendGridWebhook, T1, T2, T3, Task> handler)
			=> builder.HandleAsync(handler);

		public static WebhookReceiverBuilder<SendGridWebhook> HandleSendGrid(this WebhookReceiverBuilder<SendGridWebhook> builder, Action<SendGridWebhook> handler)
			=> builder.Handle(handler);

		public static WebhookReceiverBuilder<SendGridWebhook> HandleSendGrid<T1>(this WebhookReceiverBuilder<SendGridWebhook> builder, Action<SendGridWebhook, T1> handler)
			=> builder.Handle(handler);

		public static WebhookReceiverBuilder<SendGridWebhook> HandleSendGrid<T1, T2>(this WebhookReceiverBuilder<SendGridWebhook> builder, Action<SendGridWebhook, T1, T2> handler)
			=> builder.Handle(handler);

		public static WebhookReceiverBuilder<SendGridWebhook> HandleSendGrid<T1, T2, T3>(this WebhookReceiverBuilder<SendGridWebhook> builder, Action<SendGridWebhook, T1, T2, T3> handler)
			=> builder.Handle(handler);

		public static WebhookReceiverBuilder<SendGridEmail> HandleSendGridEmail<THandler>(this WebhookReceiverBuilder<SendGridEmail> builder)
			where THandler : class, IWebhookHandler<SendGridEmail>
			=> builder.AddHandler<THandler>();

		public static WebhookReceiverBuilder<SendGridEmail> HandleSendGridEmail(this WebhookReceiverBuilder<SendGridEmail> builder, Func<SendGridEmail, Task> handler)
			=> builder.HandleAsync(handler);

		public static WebhookReceiverBuilder<SendGridEmail> HandleSendGridEmail<T1>(this WebhookReceiverBuilder<SendGridEmail> builder, Func<SendGridEmail, T1, Task> handler)
			=> builder.HandleAsync(handler);

		public static WebhookReceiverBuilder<SendGridEmail> HandleSendGridEmail<T1, T2>(this WebhookReceiverBuilder<SendGridEmail> builder, Func<SendGridEmail, T1, T2, Task> handler)
			=> builder.HandleAsync(handler);

		public static WebhookReceiverBuilder<SendGridEmail> HandleSendGridEmail<T1, T2, T3>(this WebhookReceiverBuilder<SendGridEmail> builder, Func<SendGridEmail, T1, T2, T3, Task> handler)
			=> builder.HandleAsync(handler);

		public static WebhookReceiverBuilder<SendGridEmail> HandleSendGridEmail(this WebhookReceiverBuilder<SendGridEmail> builder, Action<SendGridEmail> handler)
			=> builder.Handle(handler);

		public static WebhookReceiverBuilder<SendGridEmail> HandleSendGridEmail<T1>(this WebhookReceiverBuilder<SendGridEmail> builder, Action<SendGridEmail, T1> handler)
			=> builder.Handle(handler);

		public static WebhookReceiverBuilder<SendGridEmail> HandleSendGridEmail<T1, T2>(this WebhookReceiverBuilder<SendGridEmail> builder, Action<SendGridEmail, T1, T2> handler)
			=> builder.Handle(handler);

		public static WebhookReceiverBuilder<SendGridEmail> HandleSendGridEmail<T1, T2, T3>(this WebhookReceiverBuilder<SendGridEmail> builder, Action<SendGridEmail, T1, T2, T3> handler)
			=> builder.Handle(handler);
	}
}
