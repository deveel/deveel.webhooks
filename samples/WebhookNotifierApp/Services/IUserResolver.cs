using Deveel.Webhooks.Models;

namespace Deveel.Webhooks.Services {
	public interface IUserResolver {
		Task<User?> ResolveUserAsync(string userId, CancellationToken cancellationToken = default);
	}
}
