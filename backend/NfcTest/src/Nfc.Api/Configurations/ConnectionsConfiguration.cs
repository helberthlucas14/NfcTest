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
            return services;
        }
        private static IServiceCollection AddDbConnection(
             this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var connectionString = configuration
                .GetConnectionString("NfeDb");
            services.AddDbContext<NfcDbContext>(
                options => options.UseSqlServer(
                    connectionString
                )
            );
            return services;
        }
    }
}
