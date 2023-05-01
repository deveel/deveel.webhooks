using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public class DefaultMongoWebhookConverter<TWebhook> : IMongoWebhookConverter<TWebhook>
		where TWebhook : class {

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
