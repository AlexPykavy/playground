using Demo.Azure.AppInsights.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApplicationInsightsOptions>(builder.Configuration.GetSection("ApplicationInsights"));
builder.Services.Configure<AzureAdOptions>(builder.Configuration.GetSection("AzureAd"));

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
