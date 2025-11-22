using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nfc.Application.Behaviors;
using Nfc.Application.Export;
using Nfc.Application.Export.Exporters;
using Nfc.Application.Export.Interfaces;
using Nfc.Application.Logging;
using Nfc.Application.Services;
using Nfc.Application.UseCases.NotaFiscal.Common;
using Nfc.Application.UseCases.NotaFiscal.CriarNotaFiscal;
using Nfc.Application.UseCases.NotaFiscal.DeleteById;
using Nfc.Application.UseCases.NotaFiscal.GetAll;
using Nfc.Application.UseCases.NotaFiscal.GetById;
using Nfc.Application.UseCases.NotaFiscal.UpdateNotaFiscal;
using Nfc.Domain.Interfaces.Repositories;
using Nfc.Domain.Interfaces.Services;
using Nfc.Infra.Data.EF;
using Nfc.Infra.Data.EF.Repositories;
using Nfc.Infra.HangFire;
using Nfc.Infra.HangFire.Jobs;
using Nfc.Infra.Data.Redis;
using StackExchange.Redis;
using Nfc.Infra.Observability;
using Nfc.Application.UseCases.Export.ExportNotaFiscal;
using Nfc.Application.UseCases.Export.GetExportStatusByJobId;
using Nfc.Infra.Storage;

namespace Nfc.Infra.CrossCutting.IoC
{
    public static class NativeInjectionBootStrapper
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterApplicationServices(services);
            RegisterDomainServices(services);
            RegisterInfraService(services, configuration);
            return services;
        }

        public static IServiceCollection RegisterInfraService(
            IServiceCollection services,
            IConfiguration configuration)
        {
            AddAppConections(services, configuration);
            services.AddScoped<INotaFiscalRepository, NotaFiscalRepository>();
            services.AddScoped<IExportStatusRepository, RedisExportStatusRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<IApplicationLogging, ApplicationLogging>();

            services.AddExportFileStorage(configuration);
            services.AddScoped<IExporter, JsonExporter>();
            services.AddScoped<IExporter, TextExporter>();
            services.AddScoped<IExportFactory, ExportFactory>();
            services.AddScoped<IExportNotasFiscalService, ExportNotasFiscalService>();
            services.AddScoped<ICorrelationContext, CorrelationContext>();
            services.AddScoped<IExportScheduler, ExportScheduler>();
            services.AddScoped<ExportJob>();

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CorrelationIdBehavior<,>));
            return services;
        }

        private static IServiceCollection AddAppConections(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSqlDbRegistration(configuration);
            services.AddRedisDbRegistration(configuration);
            services.AddHangfireInfrastructure(configuration);
            services.AddObservability(configuration);
            return services;
        }
        private static IServiceCollection AddSqlDbRegistration(
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

        private static IServiceCollection AddRedisDbRegistration(
         this IServiceCollection services,
        IConfiguration configuration
    )
        {
            var redisConnection = configuration.GetConnectionString("RedisDb");
            ArgumentNullException.ThrowIfNull(redisConnection);
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnection));
            return services;
        }

        private static IServiceCollection RegisterDomainServices(this IServiceCollection services)
        {
            services.AddScoped<INotaFiscalService, NotaFiscalService>();

            return services;
        }

        private static IServiceCollection RegisterApplicationServices(IServiceCollection services)
        {
            services.AddScoped<IRequestHandler<CriarNotaFiscalCommand, NotaFiscalResponse>, CriarNotaFiscalCommandHandler>();
            services.AddScoped<IRequestHandler<GetByIdQuery, NotaFiscalResponse>, GetByIdQueryHandler>();
            services.AddScoped<IRequestHandler<UpdateNotaFiscalCommand, NotaFiscalResponse>, UpdateNotaFiscalCommandHandler>();
            services.AddScoped<IRequestHandler<GetAllQuery, PagedList<NotaFiscalResponse>>, GetAllQueryHandler>();
            services.AddScoped<IRequestHandler<DeleteByIdCommand, Unit>, DeleteByIdCommandHandler>();
            services.AddScoped<IRequestHandler<ExportNotaFiscalCommand, string>, ExportNotaFiscalCommandHandler>();
            services.AddScoped<IRequestHandler<GetExportStatusByJobIdQuery, ExportStatus>, GetExportStatusByJobIdQueryHandler>();


            return services;
        }
    }
}
