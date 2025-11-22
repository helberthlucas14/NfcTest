using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nfc.Application.Export.Interfaces;

namespace Nfc.Infra.Storage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddExportFileStorage(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ExportFileStorageOptions>(configuration.GetSection("ExportFileStorage"));

            services.AddSingleton<IExportFileStorage>(sp =>
            {
                var optsAccessor = sp.GetRequiredService<IOptions<ExportFileStorageOptions>>();
                var opts = optsAccessor.Value;
                var env = sp.GetRequiredService<Microsoft.Extensions.Hosting.IHostEnvironment>();
                var provider = (opts.Provider ?? "S3").ToLowerInvariant();

                var local = new LocalExportFileStorage(optsAccessor, env);

                if (provider == "s3")
                {
                    var s3 = new S3ExportFileStorage(optsAccessor);
                    return new FallbackExportFileStorage(s3, local);
                }
                return local;
            });

            return services;
        }
    }
}