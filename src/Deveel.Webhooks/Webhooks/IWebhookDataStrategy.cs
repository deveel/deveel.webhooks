using System;

namespace Deveel.Webhooks {
	public interface IWebhookDataStrategy {
		/// <summary>
		/// Gets the first data factory that handles the given event
		/// </summary>
		/// <param name="eventInfo">The descriptor of the event that occurred</param>
		/// <returns>
		/// Returns an instance of <see cref="IWebhookDataFactory"/> that handles
		/// the given event, or <strong>null</strong> if none factories mathed
		/// </returns>
		IWebhookDataFactory GetDataFactory(EventInfo eventInfo);
	}
}
