using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using MongoDB.Bson;

namespace Deveel.Webhooks {
	public class MongoDbWebhookDeliveryResultLogger<TWebhook, TResult> : IWebhookDeliveryResultLogger<TWebhook>
		where TWebhook : class, IWebhook
		where TResult : MongoDbWebhookDeliveryResult {
		public MongoDbWebhookDeliveryResultLogger(
			MongoDbWebhookDeliveryResultStoreProvider<TResult> storeProvider, 
			ILogger<MongoDbWebhookDeliveryResultLogger<TWebhook, TResult>> logger) {
			StoreProvider = storeProvider;
			Logger = logger;
		}

		protected MongoDbWebhookDeliveryResultStoreProvider<TResult> StoreProvider { get; }

		protected ILogger<MongoDbWebhookDeliveryResultLogger<TWebhook, TResult>> Logger { get; }

		protected virtual TResult CreateNewResult() {
			return Activator.CreateInstance<TResult>();
		}

		protected virtual TResult ConvertResult(WebhookDeliveryResult<TWebhook> result) {
			var obj = CreateNewResult();

			obj.Webhook = ConvertWebhook(result.Webhook);
			obj.DeliveryAttempts = result.Attempts?.Select(ConvertDeliveryAttempt).ToList();

			return obj;
		}

		private MongoDbWebhookDeliveryAttempt ConvertDeliveryAttempt(WebhookDeliveryAttempt attempt) {
			return new MongoDbWebhookDeliveryAttempt {
				StartedAt = attempt.StartedAt,
				EndedAt = attempt.CompletedAt,
				ResponseStatusCode = attempt.ResponseCode,
				ResponseMessage = attempt.ResponseMessage
			};
		}

		protected virtual MongoDbWebhook ConvertWebhook(IWebhook webhook) {
			return new MongoDbWebhook {
				WebhookId = webhook.Id,
				EventType = webhook.EventType,
				SubscriptionId = webhook.SubscriptionId,
				DestinationUrl = webhook.DestinationUrl,
				Name = webhook.Name,
				// TODO: should we remove this value?
				Secret = webhook.Secret,
				Format = webhook.Format,
				Headers = webhook.Headers?.ToDictionary(x => x.Key, y => y.Value),
				Data = ConvertWebhookData(webhook.Data),
				TimeStamp = webhook.TimeStamp
			};
		}

		protected virtual BsonDocument ConvertWebhookData(object data) {
			// this is tricky: we try some possible options...

			if (data is null)
				return new BsonDocument();

			IDictionary<string, object> dictionary;

			if (data is IDictionary<string, object>) {
				dictionary = (IDictionary<string, object>)data;
			} else {
				dictionary = data.GetType()
					.GetProperties()
					.ToDictionary(x => x.Name, y => y.GetValue(data));
			}

			var document = new BsonDocument();

			// we hope here that the values are supported objects
			foreach (var item in dictionary) {
				document[item.Key] = ConvertValue(item.Value);
			}

			return document;
		}

		protected virtual BsonValue ConvertValue(object value) {
			if (value is DateTimeOffset)
				value = ((DateTimeOffset)value).DateTime;

			return BsonValue.Create(value);
		}

		public async Task LogResultAsync(string tenantId, WebhookDeliveryResult<TWebhook> result, CancellationToken cancellationToken) {
			if (result is null) 
				throw new ArgumentNullException(nameof(result));

			Logger.LogTrace("Logging the result of the delivery of a webhook of event '{EventType}' for tenant '{TenantId}'",
				result.Webhook.EventType, tenantId);

			try {
				var resultObj = ConvertResult(result);

				await StoreProvider.GetStore(tenantId).CreateAsync(resultObj, cancellationToken);
			} catch (Exception ex) {
				Logger.LogError(ex, "Could not log the result of the delivery of the Webhook of type '{EventType}' for tenant '{TenantId}' because of an error",
					result.Webhook.EventType, tenantId);
			}
		}
	}
}
