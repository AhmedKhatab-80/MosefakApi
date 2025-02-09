namespace MosefakApp.API.Extensions
{
    public static class SerilogExtensions
    {
        public static IHostBuilder UseCustomSerilog(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseSerilog((context, loggerConfig) =>
                loggerConfig.ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .Filter.ByExcluding(logEvent =>
                        logEvent.Properties.Any(p =>
                            p.Value.ToString().Contains("password") || p.Value.ToString().Contains("token"))));
        }
    }
}
