namespace MosefakApp.API.Extensions
{
    public static class IdentityConfig
    {
        public static IServiceCollection RegisterIdentityConfig(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;

                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

                options.Password.RequiredLength = 8;
            })
                  .AddEntityFrameworkStores<AppIdentityDbContext>()
                  .AddTokenProvider<DataProtectorTokenProvider<AppUser>>(TokenOptions.DefaultProvider);


            return services;
        }
    }
}
