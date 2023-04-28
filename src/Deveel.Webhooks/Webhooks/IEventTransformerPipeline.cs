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
