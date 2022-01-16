using System;

using MongoDB.Bson;

namespace Deveel.Webhooks {
	interface IMongoDocument {
		ObjectId Id { get; }
	}
}
