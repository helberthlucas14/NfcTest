using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nfc.Application.Behaviors;
using Nfc.Application.Logging;
using Nfc.Application.Services;
using Nfc.Application.UseCases.NotaFiscal.Common;
using Nfc.Application.UseCases.NotaFiscal.CriarNotaFiscal;
using Nfc.Application.UseCases.NotaFiscal.GetById;
using Nfc.Domain.Interfaces.Repositories;
using Nfc.Domain.Interfaces.Services;
using Nfc.Infra.Data.EF;
using Nfc.Infra.Data.EF.Repositories;

namespace Nfc.Infra.CrossCutting.IoC
{
    public static class NativeInjectionBootStrapper
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            RegisterInfraService(services);
            RegisterApplicationServices(services);
            RegisterDomainServices(services);
            return services;
        }

        public static IServiceCollection RegisterInfraService(IServiceCollection services)
        {
            services.AddScoped<INotaFiscalRepository, NotaFiscalRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static IServiceCollection RegisterDomainServices(this IServiceCollection services)
        {
            services.AddScoped<ICorrelationContext, CorrelationContext>();
            services.AddSingleton<IApplicationLogging, ApplicationLogging>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

            services.AddScoped<INotaFiscalService, NotaFiscalService>();

            return services;
        }

        public static IServiceCollection RegisterApplicationServices(IServiceCollection services)
        {
            services.AddScoped<IRequestHandler<CriarNotaFiscalCommand, NotaFiscalResponse>, CriarNotaFiscalCommandHandler>();
            services.AddScoped<IRequestHandler<GetByIdQuery, NotaFiscalResponse>, GetByIdQueryHandler>();


            return services;
        }
    }
}
