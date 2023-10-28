namespace Deveel {
	public static class ListExtensions {
		public static T Random<T>(this IReadOnlyList<T> list, Func<T, bool>? predicate = null, int maxRetries = 100) {
			ArgumentNullException.ThrowIfNull(list, nameof(list));

			if (list.Count == 0)
				throw new ArgumentException("The list is empty", nameof(list));

			if (maxRetries <= 0)
				throw new ArgumentOutOfRangeException(nameof(maxRetries), maxRetries, "The maximum number of retries must be greater than zero");

			var retries = 0;

			while (retries < maxRetries) {
				var item = list[System.Random.Shared.Next(0, list.Count)];

				if (predicate == null || predicate(item))
					return item;

				retries++;
			}

			throw new InvalidOperationException("Could not find a random item in the list");
		}
	}
}
