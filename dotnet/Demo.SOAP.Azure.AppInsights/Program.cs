using Microsoft.ApplicationInsights.Extensibility;
using Demo.SOAP.Azure.AppInsights.Telemetry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITelemetryInitializer, SoapTelemetryInitializer>();

// Add services to the container.

builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
