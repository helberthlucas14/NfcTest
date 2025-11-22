using Nfc.Infra.CrossCutting.IoC;
using Nfc.Infra.HangFire;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services
    .RegisterServices(builder.Configuration)
    .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly))
    .AddHttpContextAccessor()
    .AddControllers()

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
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("CORS");

//app.UseHttpsRedirection();

app.UseAuthorization();

app.UseHangfireDashboardUI();

app.MapControllers();
app.MapHub<Nfc.Api.Hubs.ExportStatusHub>("/hubs/export-status");

app.Run();

