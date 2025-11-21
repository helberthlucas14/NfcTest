using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nfc.Application.Behaviors;
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

namespace Nfc.Infra.CrossCutting.IoC
{
    public static class NativeInjectionBootStrapper
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterInfraService(services, configuration);
            RegisterApplicationServices(services);
            RegisterDomainServices(services);
            return services;
        }

        public static IServiceCollection RegisterInfraService(
            IServiceCollection services,
            IConfiguration configuration)
        {
            AddAppConections(services, configuration);
            services.AddScoped<INotaFiscalRepository, NotaFiscalRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }

        private static IServiceCollection AddAppConections(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSqlDbRegistration(configuration);
            services.AddHangfireInfrastructure(configuration);

            services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
            });
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

        private static IServiceCollection RegisterDomainServices(this IServiceCollection services)
        {
            services.AddScoped<ICorrelationContext, CorrelationContext>();
            services.AddSingleton<IApplicationLogging, ApplicationLogging>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

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


            return services;
        }
    }
}
