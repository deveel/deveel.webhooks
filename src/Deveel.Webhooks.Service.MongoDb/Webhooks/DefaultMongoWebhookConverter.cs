﻿namespace Deveel.Webhooks {
	/// <summary>
	/// A default implementation of <see cref="IMongoWebhookConverter{TWebhook}"/> that
	/// attempts to convert the given webhook object into a <see cref="MongoWebhook"/>
	/// </summary>
	/// <typeparam name="TWebhook">
	/// The type of the webhook to convert into a <see cref="MongoWebhook"/>.
	/// </typeparam>
	/// <remarks>
	/// This implementation is quite straightforward and it is configured to simply
	/// map the properties and data of a given event and webhook object to be stored
	/// into a <see cref="MongoWebhook"/> object.
	/// </remarks>
	public class DefaultMongoWebhookConverter<TWebhook> : IMongoWebhookConverter<TWebhook>
		where TWebhook : class {

		/// <summary>
		/// Converts the given <see cref="EventInfo"/> and the webhook object into
		/// a <see cref="MongoWebhook"/> object.
		/// </summary>
		/// <param name="eventInfo">
		/// The event information that is being notified.
		/// </param>
		/// <param name="webhook">
		/// The webhook that was notified to the subscribers.
		/// </param>
		/// <returns>
		/// Returns an instance of <see cref="MongoWebhook"/> that represents the
		/// webhook that can be stored into the database.
		/// </returns>
		public MongoWebhook ConvertWebhook(EventInfo eventInfo, TWebhook webhook) {
			if (webhook is IWebhook obj) {
				return new MongoWebhook {
					WebhookId = obj.Id,
					EventType = obj.EventType,
					Data = BsonValueUtil.ConvertData(obj.Data),
					TimeStamp = obj.TimeStamp
				};
			}

			return new MongoWebhook {
				EventType = eventInfo.EventType,
				TimeStamp = eventInfo.TimeStamp,
				WebhookId = eventInfo.Id,
				Data = BsonValueUtil.ConvertData(webhook)
			};
		}
	}
}
