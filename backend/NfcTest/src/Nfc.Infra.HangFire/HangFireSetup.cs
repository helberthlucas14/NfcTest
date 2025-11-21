using Hangfire;
using Hangfire.Redis.StackExchange;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nfc.Infra.HangFire
{
    public static class HangfireSetup
    {
        public static IServiceCollection AddHangfireInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var redis = configuration.GetConnectionString("RedisDb");
            ArgumentNullException.ThrowIfNull(redis);
            services.AddHangfire(config =>
            {
                config
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseRedisStorage(redis, new RedisStorageOptions
                    {
                        Prefix = "hangfire:"
                    });
            });

            services.AddHangfireServer();

            return services;
        }

        public static IApplicationBuilder UseHangfireDashboardUI(
            this IApplicationBuilder app)
        {
            app.UseHangfireDashboard("/hangfire");
            return app;
        }
    }
}
