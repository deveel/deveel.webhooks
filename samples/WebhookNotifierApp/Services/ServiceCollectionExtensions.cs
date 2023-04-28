namespace Deveel.Webhooks.Services {
	public static class ServiceCollectionExtensions {
		public static IServiceCollection AddUserStore(this IServiceCollection services, UserStore userStore) {
			services.AddSingleton<IUserResolver>(userStore);
			services.AddSingleton(userStore);

			return services;
		}

		public static IServiceCollection AddUserStore(this IServiceCollection services, Action<UserStore> configure) {
			var userStore = new UserStore();
			configure?.Invoke(userStore);
			return services.AddUserStore(userStore);
		}
	}
}
