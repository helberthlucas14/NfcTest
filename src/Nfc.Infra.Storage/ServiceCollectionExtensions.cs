using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nfc.Application.Export.Storage;

namespace Nfc.Infra.Storage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddExportFileStorage(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ExportFileStorageOptions>(configuration.GetSection("ExportFileStorage"));

            services.AddSingleton<IExportFileStorage>(sp =>
            {
                var opts = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ExportFileStorageOptions>>().Value;
                var provider = (opts.Provider ?? "Local").Trim();
                return provider.Equals("S3", StringComparison.OrdinalIgnoreCase)
                    ? new S3ExportFileStorage(sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ExportFileStorageOptions>>())
                    : new LocalExportFileStorage(sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ExportFileStorageOptions>>());
            });

            return services;
        }
    }
}