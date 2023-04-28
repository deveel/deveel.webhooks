using Deveel.Webhooks.Models;

namespace Deveel.Webhooks.Services {
	public class UserStore : IUserResolver {
		private readonly List<User> _users;

		public UserStore() {
			_users = new List<User>();
		}

		public void AddUser(User user) {
			_users.Add(user);
		}

		Task<User?> IUserResolver.ResolveUserAsync(string userId, CancellationToken cancellationToken) {
			var user = _users.FirstOrDefault(x => x.Id == userId);
			return Task.FromResult(user);
		}
	}
}
