// Copyright 2022-2023 Deveel
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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using MongoDB.Bson;

namespace Deveel.Webhooks {
	public class MongoDbWebhookDeliveryResultLogger<TWebhook, TResult> : IWebhookDeliveryResultLogger<TWebhook>
		where TWebhook : class, IWebhook
		where TResult : MongoWebhookDeliveryResult, new() {
		public MongoDbWebhookDeliveryResultLogger(
			MongoDbWebhookDeliveryResultStoreProvider<TResult> storeProvider, 
			ILogger<MongoDbWebhookDeliveryResultLogger<TWebhook, TResult>>? logger = null) {
			StoreProvider = storeProvider;
			Logger = logger ?? NullLogger<MongoDbWebhookDeliveryResultLogger<TWebhook, TResult>>.Instance;
		}

		protected MongoDbWebhookDeliveryResultStoreProvider<TResult> StoreProvider { get; }

		protected ILogger<MongoDbWebhookDeliveryResultLogger<TWebhook, TResult>> Logger { get; }

		protected virtual TResult ConvertResult(IWebhookSubscription subscription, WebhookDeliveryResult<TWebhook> result) {
			var obj = new TResult();

			obj.TenantId = subscription.TenantId;
			obj.Receiver = CreateReceiver(subscription);
			obj.Webhook = ConvertWebhook(result.Webhook);
			obj.DeliveryAttempts = result.Attempts?.Select(ConvertDeliveryAttempt).ToList() 
				?? new List<MongoWebhookDeliveryAttempt>();

			return obj;
		}

		private MongoWebhookReceiver CreateReceiver(IWebhookSubscription subscription) {
			return new MongoWebhookReceiver {
				SubscriptionId = subscription.SubscriptionId,
				SubscriptionName = subscription.Name,
				DestinationUrl = subscription.DestinationUrl,
				// TODO: body format and headers
			};
		}

		private MongoWebhookDeliveryAttempt ConvertDeliveryAttempt(WebhookDeliveryAttempt attempt) {
			return new MongoWebhookDeliveryAttempt {
				StartedAt = attempt.StartedAt,
				EndedAt = attempt.CompletedAt,
				ResponseStatusCode = attempt.ResponseCode,
				ResponseMessage = attempt.ResponseMessage
			};
		}

		protected virtual MongoWebhook ConvertWebhook(IWebhook webhook) {
			return new MongoWebhook {
				WebhookId = webhook.Id,
				EventType = webhook.EventType,
				Data = ConvertWebhookData(webhook.Data),
				TimeStamp = webhook.TimeStamp
			};
		}

		protected virtual BsonDocument ConvertWebhookData(object data) {
			// this is tricky: we try some possible options...

			if (data is null)
				return new BsonDocument();

			IDictionary<string, object?> dictionary;

			if (data is IDictionary<string, object?>) {
				dictionary = (IDictionary<string, object?>)data;
			} else {
				// TODO: make this recursive ...
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

		protected virtual BsonValue ConvertValue(object? value) {
			if (value is null)
				return BsonNull.Value;

			if (value is DateTimeOffset)
				value = ((DateTimeOffset)value).DateTime;

			return BsonValue.Create(value);
		}

		public async Task LogResultAsync(IWebhookSubscription subscription, WebhookDeliveryResult<TWebhook> result, CancellationToken cancellationToken) {
			if (result is null) 
				throw new ArgumentNullException(nameof(result));
			if (subscription is null) 
				throw new ArgumentNullException(nameof(subscription));

			// TODO: we should support also non-multi-tenant scenarios...
			if (String.IsNullOrWhiteSpace(subscription.TenantId))
				throw new ArgumentException("The tenant identifier of the subscription is not set", nameof(subscription));

			Logger.LogTrace("Logging the result of the delivery of a webhook of event '{EventType}' for tenant '{TenantId}'",
				result.Webhook.EventType, subscription.TenantId);

			try {
				var resultObj = ConvertResult(subscription, result);

				await StoreProvider.GetTenantStore(subscription.TenantId).CreateAsync(resultObj, cancellationToken);
			} catch (Exception ex) {
				Logger.LogError(ex, "Could not log the result of the delivery of the Webhook of type '{EventType}' for tenant '{TenantId}' because of an error",
					result.Webhook.EventType, subscription.TenantId);
			}
		}
	}
}
