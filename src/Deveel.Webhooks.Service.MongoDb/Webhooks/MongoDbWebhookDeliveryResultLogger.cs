// Copyright 2022-2024 Antonello Provenzano
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

using System.Collections;
using System.Reflection;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using MongoDB.Bson;

namespace Deveel.Webhooks {
	/// <summary>
	/// An implementation of <see cref="IWebhookDeliveryResultLogger{TWebhook}"/> that
	/// is backed by a MongoDB database.
	/// </summary>
	/// <typeparam name="TWebhook">
	/// The type of webhook for which the delivery results are logged.
	/// </typeparam>
	/// <typeparam name="TResult">
	/// The type of delivery result to log.
	/// </typeparam>
	public class MongoDbWebhookDeliveryResultLogger<TWebhook, TResult> : IWebhookDeliveryResultLogger<TWebhook>
		where TWebhook : class
		where TResult : MongoWebhookDeliveryResult, new() {
		private readonly IMongoWebhookConverter<TWebhook>? webhookConverter;

		/// <summary>
		/// Constructs the logger with the given store provider
		/// used to resolve the MongoDB storage for the log.
		/// </summary>
		/// <param name="storeProvider">
		/// The provider of the store used to resolve the MongoDB storage
		/// where to log the delivery results.
		/// </param>
		/// <param name="webhookConverter">
		/// A service that is used to convert the webhook object to a MongoDB
		/// compatible object for storage.
		/// </param>
		/// <param name="logger">
		/// An optional logger to use to log messages emitted by this service.
		/// </param>
		public MongoDbWebhookDeliveryResultLogger(
			IWebhookDeliveryResultRepositoryProvider<TResult> storeProvider, 
			IMongoWebhookConverter<TWebhook>? webhookConverter = null,
			ILogger<MongoDbWebhookDeliveryResultLogger<TWebhook, TResult>>? logger = null) {
			RepositoryProvider = storeProvider;
            this.webhookConverter = webhookConverter;
            Logger = logger ?? NullLogger<MongoDbWebhookDeliveryResultLogger<TWebhook, TResult>>.Instance;
		}

		/// <summary>
		/// Gets the provider used to resolve the MongoDB storage where to log
		/// the delivery results.
		/// </summary>
		protected IWebhookDeliveryResultRepositoryProvider<TResult> RepositoryProvider { get; }

		/// <summary>
		/// Gets the logger used to log messages emitted by this service.
		/// </summary>
		protected ILogger Logger { get; }

		/// <summary>
		/// Converts the given result to an object that can be stored in the
		/// MongoDB database collection.
		/// </summary>
		/// <param name="eventInfo">
		/// The information about the event that triggered the delivery of the webhook.
		/// </param>
		/// <param name="subscription">
		/// The subscription for which the delivery result is logged.
		/// </param>
		/// <param name="result">
		/// The result of the delivery of a webhook.
		/// </param>
		/// <returns>
		/// Returns an object that can be stored in the MongoDB database collection.
		/// </returns>
		protected virtual TResult ConvertResult(EventInfo eventInfo, IWebhookSubscription subscription, WebhookDeliveryResult<TWebhook> result) {
			var obj = new TResult();

			obj.TenantId = subscription.TenantId;
			obj.OperationId = result.OperationId;
			obj.EventInfo = CreateEvent(eventInfo);
			obj.Receiver = CreateReceiver(subscription);
			obj.Webhook = ConvertWebhook(eventInfo, result.Webhook);
			obj.DeliveryAttempts = result.Attempts?.Select(ConvertDeliveryAttempt).ToList() 
				?? new List<MongoWebhookDeliveryAttempt>();

			return obj;
		}

		/// <summary>
		/// Converts the given webhook object to a MongoDB compatible object
		/// </summary>
		/// <param name="eventInfo">
		/// The information about the event that triggered the delivery of the webhook.
		/// </param>
		/// <returns>
		/// Returns an instance of <see cref="MongoEventInfo"/> that can be stored
		/// in a MongoDB database collection.
		/// </returns>
		protected virtual MongoEventInfo CreateEvent(EventInfo eventInfo) {
			return new MongoEventInfo {
				EventId = eventInfo.Id,
				EventType = eventInfo.EventType,
				TimeStamp = eventInfo.TimeStamp,
				Subject = eventInfo.Subject,
				DataVersion = eventInfo.DataVersion,
				EventData = BsonValueUtil.ConvertData(eventInfo.Data)
			};
		}

		/// <summary>
		/// Converts a subscription object to a receiver object that can be
		/// stored in the MongoDB database collection.
		/// </summary>
		/// <param name="subscription"></param>
		/// <returns>
		/// Returns an instance of <see cref="MongoWebhookReceiver"/> that can be
		/// stored in the MongoDB database collection.
		/// </returns>
		protected virtual MongoWebhookReceiver CreateReceiver(IWebhookSubscription subscription) {
			return new MongoWebhookReceiver {
				SubscriptionId = subscription.SubscriptionId,
				SubscriptionName = subscription.Name,
				DestinationUrl = subscription.DestinationUrl,
				// TODO: body format and headers
			};
		}

		/// <summary>
		/// Converts the given delivery attempt to an object that can be stored
		/// </summary>
		/// <param name="attempt">
		/// The delivery attempt to convert.
		/// </param>
		/// <returns>
		/// Returns an object that can be stored in the MongoDB database collection.
		/// </returns>
		protected virtual MongoWebhookDeliveryAttempt ConvertDeliveryAttempt(WebhookDeliveryAttempt attempt) {
			return new MongoWebhookDeliveryAttempt {
				StartedAt = attempt.StartedAt,
				EndedAt = attempt.CompletedAt,
				ResponseStatusCode = attempt.ResponseCode,
				ResponseMessage = attempt.ResponseMessage
			};
		}

		/// <summary>
		/// Converts the given webhook to an object that can be stored in the
		/// MongoDB database collection.
		/// </summary>
		/// <param name="eventInfo">
		/// The information about the event that triggered the delivery of the webhook.
		/// </param>
		/// <param name="webhook">
		/// The instance of the webhook to convert.
		/// </param>
		/// <returns>
		/// Returns an object that can be stored in the MongoDB database collection.
		/// </returns>
		/// <exception cref="NotSupportedException">
		/// Thrown when the given type of webhook is not supported by this instance and
		/// no converter was provided.
		/// </exception>
		protected virtual MongoWebhook ConvertWebhook(EventInfo eventInfo, TWebhook webhook) {
			if (webhookConverter != null)
				return webhookConverter.ConvertWebhook(eventInfo, webhook);

			throw new NotSupportedException("The given type of webhook is not supported by this instance of the logger");
		}

		/// <inheritdoc/>
		public async Task LogResultAsync(EventInfo eventInfo, IWebhookSubscription subscription, WebhookDeliveryResult<TWebhook> result, CancellationToken cancellationToken) {
			ArgumentNullException.ThrowIfNull(result, nameof(result));
			ArgumentNullException.ThrowIfNull(subscription, nameof(subscription));

			// TODO: we should support also non-multi-tenant scenarios...
			if (String.IsNullOrWhiteSpace(subscription.TenantId))
				throw new ArgumentException("The tenant identifier of the subscription is not set", nameof(subscription));

			Logger.LogTrace("Logging the result of the delivery of a webhook of type '{WebhookType}' for tenant '{TenantId}'",
				typeof(TWebhook), subscription.TenantId);

			try {
				var resultObj = ConvertResult(eventInfo, subscription, result);

				var repository = await RepositoryProvider.GetRepositoryAsync(subscription.TenantId, cancellationToken);

				await repository.AddAsync(resultObj, cancellationToken);
			} catch (Exception ex) {
				Logger.LogError(ex, "Could not log the result of the delivery of the Webhook of type '{WebhookType}' for tenant '{TenantId}' because of an error",
					typeof(TWebhook), subscription.TenantId);
			}
		}
	}
}
