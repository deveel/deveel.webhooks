using System;

using MongoDB.Bson;

namespace Deveel.Webhooks {
	static class ObjectIdExtensions {
		public static string ToEntityId(this ObjectId id)
			=> id == ObjectId.Empty ? null : id.ToString();
	}
}
