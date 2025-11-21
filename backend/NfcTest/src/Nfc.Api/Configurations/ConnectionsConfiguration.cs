using Microsoft.EntityFrameworkCore;
using Nfc.Infra.Data.EF;

namespace Nfc.Api.Configurations
{
    public static class ConnectionsConfiguration
    {
        public static IServiceCollection AddAppConections(
         this IServiceCollection services,
         IConfiguration configuration
        )
        {
            services.AddDbConnection(configuration);
            services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
            });
            return services;
        }
        private static IServiceCollection AddDbConnection(
             this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var connectionString = configuration
                .GetConnectionString("NfeDb");
            ArgumentNullException.ThrowIfNull(connectionString);
            services.AddDbContext<NfcDbContext>(
                options => options.UseSqlServer(
                    connectionString
                )
            );
            return services;
        }
    }
}
