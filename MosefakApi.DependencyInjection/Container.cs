namespace MosefakApi.DependencyInjection
{
    public static class Container
    {
        public static IServiceCollection RegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
        {

            // Register AppDbContext 

            services.RegisterConnectionString(configuration);

            // Register Redis

          //  services.RegisterRedisConfig(configuration);

            // register Identity

            services.RegisterIdentityConnectionString(configuration);


            // Register IUnit Of work 

            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            services.AddScoped(typeof(IUserRepository), typeof(UserRepository));


            //Register Services

            services.RegisterServices();

            // Register AutoMappper

            services.AddAutoMapper(typeof(Mapping));

            // Register RateLimiting

            services.RegisterConcurrencyRateLimitingConfig();

            services.RegisterOptionsPatternConfig(configuration);

            return services;
        }

        private static IServiceCollection RegisterConnectionString(this IServiceCollection services, IConfiguration configuration)
        {

            var connection = configuration["ConnectionStrings:DefaultConnectionString"];
            services.AddDbContext<AppDbContext>(x => x.UseSqlServer(connection,options=>
            {
                options.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                options.CommandTimeout(60);

            }));
            services.AddScoped<AppDbContext, AppDbContext>();
            return services;
        }


        //private static IServiceCollection RegisterRedisConfig(this IServiceCollection services, IConfiguration configuration)
        //{
        //    services.AddScoped<IConnectionMultiplexer>(options =>
        //    {
        //        var connection = configuration["ConnectionStrings:RedisConnectionString"];

        //        return ConnectionMultiplexer.Connect(connection!);
        //    });

        //    return services;
        //}

        private static IServiceCollection RegisterIdentityConnectionString(this IServiceCollection services, IConfiguration configuration)
        {

            var connection = configuration["ConnectionStrings:IdentityConnectionString"];
            services.AddDbContext<AppIdentityDbContext>(x => x.UseSqlServer(connection, options =>
            {
                options.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                options.CommandTimeout(60);
            }));
            services.AddScoped<AppIdentityDbContext, AppIdentityDbContext>();

            return services;
        }

        private static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IJwtProvider), typeof(JwtProvider));
            services.AddScoped(typeof(IAuthenticationService), typeof(AuthenticationService));
            services.AddScoped(typeof(IRoleService), typeof(RoleService));
            services.AddScoped(typeof(IUserService), typeof(UserService));
            services.AddScoped(typeof(IEmailSender), typeof(EmailSender));
            services.AddScoped(typeof(IEmailBodyBuilder), typeof(EmailBodyBuilder));
            services.AddScoped(typeof(IDoctorService), typeof(DoctorService));
            services.AddScoped(typeof(IPatientService), typeof(PatientService));
            services.AddScoped(typeof(IAppointmentService), typeof(AppointmentService));
            services.AddScoped(typeof(IImageService), typeof(ImageService));
            services.AddScoped<IIdProtectorService, IdProtectorService>();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<ILoggerService, LoggerService>();

            return services;
        }

        private static IServiceCollection RegisterOptionsPatternConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));

            return services;
        }

        private static IServiceCollection RegisterConcurrencyRateLimitingConfig(this IServiceCollection services)
        {
            services.AddRateLimiter(RateLimiterOptions =>
            {
                RateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                RateLimiterOptions.AddConcurrencyLimiter(RateLimiterType.Concurrency, ConcurrencyLimiterOptions =>
                {
                    ConcurrencyLimiterOptions.PermitLimit = 1000;
                    ConcurrencyLimiterOptions.QueueLimit = 100; // will go to waiting list..
                    ConcurrencyLimiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // will exist empty place to accept request, will move oldest waited request from Queue to execute..
                });
            });

            return services;
        }

     
    }
}
