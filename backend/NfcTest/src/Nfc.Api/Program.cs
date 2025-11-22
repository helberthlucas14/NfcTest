using Nfc.Infra.CrossCutting.IoC;
using Nfc.Infra.CrossCutting.Commons.Filters;
using Nfc.Infra.CrossCutting.Commons.Middlewares;
using Nfc.Infra.HangFire;
using Scalar.AspNetCore;
using Nfc.Infra.Observability;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

ObservabilitySetup.ConfigureSerilog(builder);

builder.Services
    .RegisterServices(builder.Configuration)
    .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly))
    .AddHttpContextAccessor()
    .AddControllers(options => options.Filters.Add<ApiGlobalExceptionFilter>())
    ;

builder.Services.AddScoped<Nfc.Application.Export.IExportStatusNotifier, Nfc.Api.Notifications.SignalRExportStatusNotifier>();

builder.Services
    .AddCors(p => p.AddPolicy("CORS", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || string.Equals(app.Environment.EnvironmentName, "Docker", StringComparison.OrdinalIgnoreCase))
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("CORS");

//app.UseHttpsRedirection();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseAuthorization();

app.UseHangfireDashboardUI();

ObservabilitySetup.UseObservability(app);


app.MapControllers();
app.MapHub<Nfc.Api.Hubs.ExportStatusHub>("/hubs/export-status");

app.Run();

